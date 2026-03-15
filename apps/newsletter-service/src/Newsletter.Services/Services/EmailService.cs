using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Options;
using Polly;
using Polly.Retry;

namespace Newsletter.Services.Services;

public class EmailService : IEmailService
{
    private readonly SmtpOptions _smtpOptions;
    private readonly ILogger<EmailService> _logger;
    private readonly ResiliencePipeline _retryPipeline;

    public EmailService(IOptions<AppOptions> options, ILogger<EmailService> logger)
    {
        _smtpOptions = options.Value.Smtp;
        _logger = logger;

        // Retry policy: 3 retries with exponential backoff
        _retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(1),
                OnRetry = args =>
                {
                    _logger.LogWarning(args.Outcome.Exception, "Email send failed. Retry attempt {AttemptNumber}",
                        args.AttemptNumber);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlContent)
    {
        await _retryPipeline.ExecuteAsync(async ct =>
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpOptions.FromName, _smtpOptions.FromEmail));
            message.To.Add(new MailboxAddress(toName ?? toEmail, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlContent };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, SecureSocketOptions.StartTls, ct);

                if (!string.IsNullOrEmpty(_smtpOptions.Username))
                {
                    await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password, ct);
                }

                await client.SendAsync(message, ct);
                await client.DisconnectAsync(true, ct);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        });
    }

    public async Task SendBulkEmailAsync(List<(string Email, string Name)> recipients, string subject, string htmlContent)
    {
        const int batchSize = 50;
        const int delayBetweenBatchesMs = 1000; // 1 second delay between batches

        _logger.LogInformation("Sending bulk email to {Count} recipients in batches of {BatchSize}",
            recipients.Count, batchSize);

        for (int i = 0; i < recipients.Count; i += batchSize)
        {
            var batch = recipients.Skip(i).Take(batchSize).ToList();

            var tasks = batch.Select(recipient =>
                SendEmailSafely(recipient.Email, recipient.Name, subject, htmlContent));

            await Task.WhenAll(tasks);

            if (i + batchSize < recipients.Count)
            {
                await Task.Delay(delayBetweenBatchesMs);
            }
        }

        _logger.LogInformation("Bulk email send completed for {Count} recipients", recipients.Count);
    }

    private async Task SendEmailSafely(string toEmail, string toName, string subject, string htmlContent)
    {
        try
        {
            await SendEmailAsync(toEmail, toName, subject, htmlContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email} after retries", toEmail);
            // Don't throw - we want to continue with other emails
        }
    }
}

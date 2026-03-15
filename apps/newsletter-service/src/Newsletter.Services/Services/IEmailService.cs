namespace Newsletter.Services.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string toName, string subject, string htmlContent);
    Task SendBulkEmailAsync(List<(string Email, string Name)> recipients, string subject, string htmlContent);
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newsletter.Services.Services;

namespace Newsletter.Services.BackgroundServices;

public class DailyNewsletterScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DailyNewsletterScheduler> _logger;
    private const int ScheduledHourUtc = 9; // 09:00 UTC

    public DailyNewsletterScheduler(IServiceScopeFactory scopeFactory, ILogger<DailyNewsletterScheduler> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DailyNewsletterScheduler started. Will send daily digest at {Hour}:00 UTC", ScheduledHourUtc);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            var nextRun = CalculateNextRunTime(now);
            var delay = nextRun - now;

            _logger.LogInformation("Next daily digest scheduled for {NextRun} UTC (in {DelayHours:F2} hours)",
                nextRun, delay.TotalHours);

            try
            {
                await Task.Delay(delay, stoppingToken);

                if (!stoppingToken.IsCancellationRequested)
                {
                    await SendDailyDigest();
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("DailyNewsletterScheduler is stopping");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DailyNewsletterScheduler");
                // Wait a bit before retrying
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task SendDailyDigest()
    {
        try
        {
            _logger.LogInformation("Triggering daily digest send");

            using var scope = _scopeFactory.CreateScope();
            var newsletterService = scope.ServiceProvider.GetRequiredService<INewsletterService>();

            await newsletterService.SendDailyDigestAsync();

            _logger.LogInformation("Daily digest sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send daily digest");
        }
    }

    private static DateTime CalculateNextRunTime(DateTime now)
    {
        var today = now.Date;
        var todayScheduled = today.AddHours(ScheduledHourUtc);

        if (now < todayScheduled)
        {
            // Today's scheduled time hasn't passed yet
            return todayScheduled;
        }
        else
        {
            // Schedule for tomorrow
            return todayScheduled.AddDays(1);
        }
    }
}

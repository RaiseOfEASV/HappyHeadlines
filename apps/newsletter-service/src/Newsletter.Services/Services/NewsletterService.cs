using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newsletter.Data.configuration;
using Newsletter.Data.entities;
using Newsletter.Services.Clients;
using Newsletter.Services.DTOs;
using Newsletter.Services.Templates;

namespace Newsletter.Services.Services;

public class NewsletterService : INewsletterService
{
    private readonly NewsletterDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ISubscriberService _subscriberService;
    private readonly IArticleClient _articleClient;
    private readonly ILogger<NewsletterService> _logger;

    public NewsletterService(
        NewsletterDbContext context,
        IEmailService emailService,
        ISubscriberService subscriberService,
        IArticleClient articleClient,
        ILogger<NewsletterService> logger)
    {
        _context = context;
        _emailService = emailService;
        _subscriberService = subscriberService;
        _articleClient = articleClient;
        _logger = logger;
    }

    public async Task SendBreakingNewsAsync(Guid articleId, string title, string content, string continent)
    {
        _logger.LogInformation("Sending breaking news for article {ArticleId}", articleId);

        var subscribers = await _subscriberService.GetActiveSubscribersAsync();
        if (!subscribers.Any())
        {
            _logger.LogWarning("No active subscribers found for breaking news");
            return;
        }

        var subject = $"BREAKING: {title}";
        var htmlContent = EmailTemplates.BreakingNewsTemplate(title, content, continent);

        // Create newsletter history
        var newsletter = new NewsletterHistoryEntity
        {
            NewsletterId = Guid.NewGuid(),
            NewsletterType = NewsletterType.BreakingNews,
            Subject = subject,
            ContentHtml = htmlContent,
            ArticleIds = JsonSerializer.Serialize(new[] { articleId }),
            SentAt = DateTime.UtcNow,
            RecipientCount = subscribers.Count,
            CreatedAt = DateTime.UtcNow
        };

        _context.NewsletterHistory.Add(newsletter);
        await _context.SaveChangesAsync();

        // Create delivery records
        var deliveries = subscribers.Select(s => new NewsletterDeliveryEntity
        {
            DeliveryId = Guid.NewGuid(),
            NewsletterId = newsletter.NewsletterId,
            SubscriberId = s.SubscriberId,
            Email = s.Email,
            Status = DeliveryStatus.Queued,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _context.NewsletterDelivery.AddRange(deliveries);
        await _context.SaveChangesAsync();

        // Send emails
        var recipients = subscribers.Select(s => (s.Email, s.Name ?? s.Email)).ToList();
        await _emailService.SendBulkEmailAsync(recipients, subject, htmlContent);

        // Update delivery status
        foreach (var delivery in deliveries)
        {
            delivery.Status = DeliveryStatus.Sent;
            delivery.SentAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();

        _logger.LogInformation("Breaking news sent to {Count} subscribers", subscribers.Count);
    }

    public async Task SendDailyDigestAsync()
    {
        _logger.LogInformation("Sending daily digest newsletter");

        var subscribers = await _subscriberService.GetActiveSubscribersAsync();
        if (!subscribers.Any())
        {
            _logger.LogWarning("No active subscribers found for daily digest");
            return;
        }

        // Fetch recent articles from ArticleService
        var articles = await _articleClient.GetRecentArticlesAsync(limit: 10);
        if (!articles.Any())
        {
            _logger.LogWarning("No recent articles found for daily digest");
            return;
        }

        var subject = $"Daily Digest - {DateTime.UtcNow:MMMM dd, yyyy}";
        var articlesForTemplate = articles.Select(a => (a.Name, a.Content, a.Continent)).ToList();
        var htmlContent = EmailTemplates.DailyDigestTemplate(articlesForTemplate);

        // Create newsletter history
        var newsletter = new NewsletterHistoryEntity
        {
            NewsletterId = Guid.NewGuid(),
            NewsletterType = NewsletterType.DailyDigest,
            Subject = subject,
            ContentHtml = htmlContent,
            ArticleIds = JsonSerializer.Serialize(articles.Select(a => a.ArticleId).ToArray()),
            SentAt = DateTime.UtcNow,
            RecipientCount = subscribers.Count,
            CreatedAt = DateTime.UtcNow
        };

        _context.NewsletterHistory.Add(newsletter);
        await _context.SaveChangesAsync();

        // Create delivery records
        var deliveries = subscribers.Select(s => new NewsletterDeliveryEntity
        {
            DeliveryId = Guid.NewGuid(),
            NewsletterId = newsletter.NewsletterId,
            SubscriberId = s.SubscriberId,
            Email = s.Email,
            Status = DeliveryStatus.Queued,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _context.NewsletterDelivery.AddRange(deliveries);
        await _context.SaveChangesAsync();

        // Send emails
        var recipients = subscribers.Select(s => (s.Email, s.Name ?? s.Email)).ToList();
        await _emailService.SendBulkEmailAsync(recipients, subject, htmlContent);

        // Update delivery status
        foreach (var delivery in deliveries)
        {
            delivery.Status = DeliveryStatus.Sent;
            delivery.SentAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();

        _logger.LogInformation("Daily digest sent to {Count} subscribers with {ArticleCount} articles",
            subscribers.Count, articles.Count);
    }

    public async Task<List<NewsletterDto>> GetHistoryAsync(int limit = 50)
    {
        var newsletters = await _context.NewsletterHistory
            .OrderByDescending(n => n.SentAt)
            .Take(limit)
            .ToListAsync();

        return newsletters.Select(n =>
        {
            var articleIds = new List<Guid>();
            try
            {
                articleIds = JsonSerializer.Deserialize<List<Guid>>(n.ArticleIds) ?? new();
            }
            catch { }

            return new NewsletterDto
            {
                NewsletterId = n.NewsletterId,
                NewsletterType = n.NewsletterType,
                Subject = n.Subject,
                RecipientCount = n.RecipientCount,
                SentAt = n.SentAt,
                ArticleIds = articleIds
            };
        }).ToList();
    }
}

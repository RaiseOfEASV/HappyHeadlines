using MassTransit;
using Microsoft.Extensions.Logging;
using Newsletter.Services.Messages;
using Newsletter.Services.Services;

namespace Newsletter.Services.Consumers;

public class ArticlePublishedConsumer : IConsumer<ArticlePublishedEvent>
{
    private readonly INewsletterService _newsletterService;
    private readonly ILogger<ArticlePublishedConsumer> _logger;

    public ArticlePublishedConsumer(INewsletterService newsletterService, ILogger<ArticlePublishedConsumer> logger)
    {
        _newsletterService = newsletterService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ArticlePublishedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received ArticlePublishedEvent for article {ArticleId}, IsBreakingNews: {IsBreaking}",
            message.ArticleId, message.IsBreakingNews);

        if (!message.IsBreakingNews)
        {
            _logger.LogInformation("Article {ArticleId} is not breaking news - skipping immediate notification", message.ArticleId);
            return;
        }

        try
        {
            await _newsletterService.SendBreakingNewsAsync(
                message.ArticleId,
                message.Name,
                message.Content,
                message.Continent);

            _logger.LogInformation("Successfully sent breaking news notification for article {ArticleId}", message.ArticleId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send breaking news notification for article {ArticleId}", message.ArticleId);
            throw; // Requeue message for retry
        }
    }
}

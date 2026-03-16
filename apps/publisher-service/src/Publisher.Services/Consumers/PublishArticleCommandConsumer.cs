using HappyHeadlines.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Publisher.Data.entities;
using Publisher.Services.Clients;
using Publisher.Services.DTOs;
using Publisher.Services.Messages;

namespace Publisher.Services.Consumers;

public class PublishArticleCommandConsumer : IConsumer<PublishArticleCommand>
{
    private readonly IPublisherService _publisherService;
    private readonly IArticleServiceClient _articleClient;
    private readonly ILogger<PublishArticleCommandConsumer> _logger;

    public PublishArticleCommandConsumer(
        IPublisherService publisherService,
        IArticleServiceClient articleClient,
        ILogger<PublishArticleCommandConsumer> logger)
    {
        _publisherService = publisherService;
        _articleClient = articleClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PublishArticleCommand> context)
    {
        var command = context.Message;
        _logger.LogInformation("Processing PublishArticleCommand for publication {PublicationId}", command.PublicationId);

        var articleResponse = await _articleClient.CreateArticleAsync(
            command.Continent,
            new ArticleCreateRequest(command.Title, command.Content, command.AuthorIds));

        if (articleResponse is null)
        {
            _logger.LogError("Failed to create article for publication {PublicationId}", command.PublicationId);
            await _publisherService.UpdatePublicationStatusAsync(
                command.PublicationId,
                new UpdatePublicationStatusDto(PublicationStatus.Failed, null, "Failed to create article in ArticleService"));
            return;
        }

        await _publisherService.UpdatePublicationStatusAsync(
            command.PublicationId,
            new UpdatePublicationStatusDto(PublicationStatus.Published, articleResponse.ArticleId));

        await context.Publish(new ArticlePublishedEvent
        {
            ArticleId = articleResponse.ArticleId,
            Name = command.Title,
            Content = command.Content,
            Timestamp = articleResponse.Timestamp,
            Continent = command.Continent,
            IsBreakingNews = false
        });

        _logger.LogInformation("Publication {PublicationId} completed, article {ArticleId} created",
            command.PublicationId, articleResponse.ArticleId);
    }
}

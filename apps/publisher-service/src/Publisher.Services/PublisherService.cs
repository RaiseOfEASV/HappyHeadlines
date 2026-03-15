using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Publisher.Data.configuration;
using Publisher.Data.entities;
using Publisher.Services.Clients;
using Publisher.Services.DTOs;
using Publisher.Services.Messages;

namespace Publisher.Services;

public class PublisherService : IPublisherService
{
    private readonly PublisherDbContext _db;
    private readonly IDraftServiceClient _draftClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<PublisherService> _logger;

    public PublisherService(
        PublisherDbContext db,
        IDraftServiceClient draftClient,
        IPublishEndpoint publishEndpoint,
        ILogger<PublisherService> logger)
    {
        _db = db;
        _draftClient = draftClient;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<PublicationDto> PublishDraftAsync(CreatePublicationDto dto)
    {
        _logger.LogInformation("Publishing draft {DraftId} by publisher {PublisherId}", dto.DraftId, dto.PublisherId);

        // 1. Validate draft exists
        var draft = await _draftClient.GetDraftByIdAsync(dto.DraftId);
        if (draft is null)
        {
            _logger.LogError("Draft {DraftId} not found", dto.DraftId);
            throw new InvalidOperationException($"Draft {dto.DraftId} not found");
        }

        // 2. Check if draft already published
        var existingPublication = await _db.Publications
            .FirstOrDefaultAsync(p => p.DraftId == dto.DraftId && p.Status == PublicationStatus.Published);

        if (existingPublication is not null)
        {
            _logger.LogWarning("Draft {DraftId} already published as article {ArticleId}", dto.DraftId, existingPublication.ArticleId);
            throw new InvalidOperationException($"Draft {dto.DraftId} has already been published");
        }

        // 3. Create publication record
        var publication = new PublicationEntity
        {
            Id = Guid.NewGuid(),
            DraftId = dto.DraftId,
            PublisherId = dto.PublisherId,
            Title = draft.Title,
            Content = draft.Content,
            Continent = dto.Continent,
            Status = PublicationStatus.Publishing,
            PublishInitiatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Publications.Add(publication);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Publication {PublicationId} created with status 'Publishing'", publication.Id);

        // 4. Publish message to RabbitMQ
        try
        {
            var command = new PublishArticleCommand
            {
                PublicationId = publication.Id,
                DraftId = dto.DraftId,
                Title = draft.Title,
                Content = draft.Content,
                PublisherId = dto.PublisherId,
                AuthorIds = new List<Guid> { draft.AuthorId },
                Continent = dto.Continent ?? "Unknown",
                RequestedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(command);

            _logger.LogInformation("PublishArticleCommand sent to queue for publication {PublicationId}", publication.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message for publication {PublicationId}", publication.Id);

            // Update status to Failed
            publication.Status = PublicationStatus.Failed;
            publication.ErrorMessage = $"Failed to send message to queue: {ex.Message}";
            publication.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            throw;
        }

        return MapToDto(publication);
    }

    public async Task<PublicationDto?> GetPublicationByIdAsync(Guid publicationId)
    {
        _logger.LogDebug("Fetching publication {PublicationId}", publicationId);

        var publication = await _db.Publications.FindAsync(publicationId);

        return publication is null ? null : MapToDto(publication);
    }

    public async Task<IEnumerable<PublicationDto>> GetPublicationsByPublisherAsync(Guid publisherId)
    {
        _logger.LogDebug("Fetching publications for publisher {PublisherId}", publisherId);

        var publications = await _db.Publications
            .Where(p => p.PublisherId == publisherId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return publications.Select(MapToDto);
    }

    public async Task<PublicationDto?> UpdatePublicationStatusAsync(Guid publicationId, UpdatePublicationStatusDto dto)
    {
        _logger.LogInformation("Updating publication {PublicationId} status to {Status}", publicationId, dto.Status);

        var publication = await _db.Publications.FindAsync(publicationId);
        if (publication is null)
        {
            _logger.LogWarning("Publication {PublicationId} not found", publicationId);
            return null;
        }

        publication.Status = dto.Status;
        publication.ArticleId = dto.ArticleId;
        publication.ErrorMessage = dto.ErrorMessage;
        publication.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == PublicationStatus.Published || dto.Status == PublicationStatus.Failed)
        {
            publication.PublishCompletedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        _logger.LogInformation("Publication {PublicationId} updated to status {Status}", publicationId, dto.Status);

        return MapToDto(publication);
    }

    public async Task<PublicationDto?> RetryPublicationAsync(Guid publicationId)
    {
        _logger.LogInformation("Retrying publication {PublicationId}", publicationId);

        var publication = await _db.Publications.FindAsync(publicationId);
        if (publication is null)
        {
            _logger.LogWarning("Publication {PublicationId} not found", publicationId);
            return null;
        }

        if (publication.Status != PublicationStatus.Failed)
        {
            _logger.LogWarning("Publication {PublicationId} is not in Failed status, cannot retry", publicationId);
            throw new InvalidOperationException($"Publication {publicationId} is not in Failed status");
        }

        // Update status and retry
        publication.Status = PublicationStatus.Publishing;
        publication.ErrorMessage = null;
        publication.PublishCompletedAt = null;
        publication.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        // Republish message
        var command = new PublishArticleCommand
        {
            PublicationId = publication.Id,
            DraftId = publication.DraftId,
            Title = publication.Title,
            Content = publication.Content,
            PublisherId = publication.PublisherId,
            AuthorIds = new List<Guid>(), // Would need to be stored or fetched
            Continent = publication.Continent ?? "Unknown",
            RequestedAt = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(command);

        _logger.LogInformation("Retry message sent for publication {PublicationId}", publicationId);

        return MapToDto(publication);
    }

    private static PublicationDto MapToDto(PublicationEntity entity)
    {
        return new PublicationDto(
            entity.Id,
            entity.DraftId,
            entity.ArticleId,
            entity.PublisherId,
            entity.Status,
            entity.Title,
            entity.Content,
            entity.Continent,
            entity.PublishInitiatedAt,
            entity.PublishCompletedAt,
            entity.ErrorMessage,
            entity.CreatedAt,
            entity.UpdatedAt
        );
    }
}

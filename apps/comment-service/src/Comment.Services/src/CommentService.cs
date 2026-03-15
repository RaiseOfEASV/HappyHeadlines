using Comment.Data.configuration;
using Comment.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Comment.Services;

public class CommentService(
    CommentDbContext db,
    IProfanityClient profanityClient,
    ICommentCacheService cache,
    ILogger<CommentService> logger) : ICommentService
{
    public async Task<IEnumerable<CommentDto>> GetByArticleAsync(Guid articleId)
    {
        // Cache-miss approach: check Redis first
        var cached = await cache.GetAsync(articleId);
        if (cached is not null)
        {
            logger.LogInformation("Cache HIT for article {ArticleId}", articleId);
            return cached;
        }

        logger.LogInformation("Cache MISS for article {ArticleId} — loading from database", articleId);

        var comments = await db.Comments
            .Where(c => c.ArticleId == articleId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        var dtos = comments.Select(ToDto).ToList();

        // Store in Redis — LRU evicts the oldest article if cache is full (30 articles)
        await cache.SetAsync(articleId, dtos);

        return dtos;
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto)
    {
        string filteredContent;
        try
        {
            filteredContent = await profanityClient.FilterAsync(dto.Content);
        }
        catch (Exception ex)
        {
            // Circuit breaker is open or ProfanityService is unavailable — swimlane isolation:
            // save the comment without filtering rather than failing the entire request.
            logger.LogWarning(ex, "ProfanityService unavailable — saving comment without profanity filtering.");
            filteredContent = dto.Content;
        }

        var entity = new CommentEntity
        {
            CommentId = Guid.NewGuid(),
            ArticleId = dto.ArticleId,
            AuthorId = dto.AuthorId,
            Content = filteredContent,
            CreatedAt = DateTime.UtcNow,
        };

        db.Comments.Add(entity);
        await db.SaveChangesAsync();

        // Invalidate cache so the next GET fetches fresh data including this comment
        await cache.InvalidateAsync(dto.ArticleId);

        return ToDto(entity);
    }

    public async Task DeleteAsync(Guid commentId)
    {
        var entity = await db.Comments.FindAsync(commentId);
        if (entity is null) return;

        db.Comments.Remove(entity);
        await db.SaveChangesAsync();

        // Invalidate cache for this article
        await cache.InvalidateAsync(entity.ArticleId);
    }

    private static CommentDto ToDto(CommentEntity e) =>
        new(e.CommentId, e.ArticleId, e.AuthorId, e.Content, e.CreatedAt);
}

using Comment.Data.configuration;
using Comment.Data.entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Comment.Services;

public class CommentService(CommentDbContext db, IProfanityClient profanityClient, ILogger<CommentService> logger,CommentsCacheService commentsCacheService) : ICommentService
{
    public async Task<IEnumerable<CommentDto>> GetByArticleAsync(Guid articleId)
    {
        var cache = await commentsCacheService.GetByArticleAsync(articleId);
        if (cache != null)
        {
            logger.LogInformation($"Getting comments for {articleId}");
            return cache.Select(c => new CommentDto(c.CommentId, c.ArticleId, c.AuthorId, c.Content, c.CreatedAt));
        }
    
        var comments = await db.Comments
            .Where(c => c.ArticleId == articleId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        var commentsDtos = comments.Select(c =>
            new CommentDto(c.CommentId, c.ArticleId, c.AuthorId, c.Content, c.CreatedAt)).ToList();
        await commentsCacheService.SetByArticleAsync(articleId, commentsDtos);
        return commentsDtos;
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

        return new CommentDto(entity.CommentId, entity.ArticleId, entity.AuthorId, entity.Content, entity.CreatedAt);
    }

    public async Task DeleteAsync(Guid commentId)
    {
        var entity = await db.Comments.FindAsync(commentId);
        if (entity is null) return;

        db.Comments.Remove(entity);
        await db.SaveChangesAsync();
    }
}

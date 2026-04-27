using Comment.Data.configuration;
using Comment.Data.entities;
using Feature.Flags;
using MessageClient.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using models;
using SharedContracts.contracts;

namespace Comment.Services;

public class CommentService(FeatureRouter featurerouter,CommentDbContext db,IMessageClient messageClient, IProfanityClient profanityClient, ILogger<CommentService> logger,CommentsCacheService commentsCacheService) : ICommentService
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


    public async Task<CommentDto> CreateAsync(CreateCommentDto dto,ConfigProfanity flags)
    {
    
    var response = await featurerouter.CreateComment(dto,flags);
    return response;
    }

    public async Task DeleteAsync(Guid commentId)
    {
        var entity = await db.Comments.FindAsync(commentId);
        if (entity is null) return;

        db.Comments.Remove(entity);
        await db.SaveChangesAsync();
        await commentsCacheService.InvalidateArticleAsync(entity.ArticleId);
    }
}

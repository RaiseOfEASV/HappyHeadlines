using Comment.Data.cache;

namespace Comment.Services;

public class CommentsCacheService(ICacheService cache)
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);

    private static string ArticleKey(Guid articleId) => $"comments:article:{articleId}";

    public Task<IEnumerable<CommentDto>?> GetByArticleAsync(Guid articleId)
        => cache.GetAsync<IEnumerable<CommentDto>>(ArticleKey(articleId));

    public Task SetByArticleAsync(Guid articleId, IEnumerable<CommentDto> comments)
        => cache.SetAsync(ArticleKey(articleId), comments, DefaultExpiry);

    public Task InvalidateArticleAsync(Guid articleId)
        => cache.RemoveAsync(ArticleKey(articleId));
}
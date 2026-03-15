using Comment.Data.cache;
using Prometheus;

namespace Comment.Services;

public class CommentsCacheService(ICacheService cache)
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(5);

    private static readonly Counter CacheHits = Metrics.CreateCounter(
        "comment_cache_hits_total", "Number of comment cache hits");

    private static readonly Counter CacheMisses = Metrics.CreateCounter(
        "comment_cache_misses_total", "Number of comment cache misses");

    private static string ArticleKey(Guid articleId) => $"comments:article:{articleId}";

    public async Task<IEnumerable<CommentDto>?> GetByArticleAsync(Guid articleId)
    {
        var result = await cache.GetAsync<IEnumerable<CommentDto>>(ArticleKey(articleId));
        if (result is not null)
            CacheHits.Inc();
        else
            CacheMisses.Inc();
        return result;
    }

    public Task SetByArticleAsync(Guid articleId, IEnumerable<CommentDto> comments)
        => cache.SetAsync(ArticleKey(articleId), comments, DefaultExpiry);

    public Task InvalidateArticleAsync(Guid articleId)
        => cache.RemoveAsync(ArticleKey(articleId));
}
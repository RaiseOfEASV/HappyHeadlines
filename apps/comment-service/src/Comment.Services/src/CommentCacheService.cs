using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Comment.Services;

// Stores comments for up to 30 most recently accessed articles in Redis.
// LRU eviction is tracked in-memory: when the 31st article arrives,
// the least-recently-used article's comments are removed from Redis.
public class CommentCacheService : ICommentCacheService
{
    private const int MaxArticles = 30;

    private readonly IDistributedCache _cache;
    private readonly ILogger<CommentCacheService> _logger;

    // LRU tracking — front = most recently used, back = least recently used
    private readonly LinkedList<Guid> _lruList = new();
    private readonly Dictionary<Guid, LinkedListNode<Guid>> _lruMap = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public CommentCacheService(IDistributedCache cache, ILogger<CommentCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<CommentDto>?> GetAsync(Guid articleId)
    {
        var data = await _cache.GetStringAsync(CacheKey(articleId));
        if (data is null)
            return null;

        // Cache hit — move this article to the front of the LRU list
        await _lock.WaitAsync();
        try { Touch(articleId); }
        finally { _lock.Release(); }

        return JsonSerializer.Deserialize<List<CommentDto>>(data);
    }

    public async Task SetAsync(Guid articleId, IEnumerable<CommentDto> comments)
    {
        Guid? evicted = null;

        await _lock.WaitAsync();
        try
        {
            // If this is a new article and cache is full — evict the LRU article
            if (!_lruMap.ContainsKey(articleId) && _lruList.Count >= MaxArticles)
            {
                evicted = _lruList.Last!.Value;
                _lruMap.Remove(evicted.Value);
                _lruList.RemoveLast();
                _logger.LogInformation("LRU evicted article {ArticleId} from comment cache", evicted);
            }

            Touch(articleId);
        }
        finally { _lock.Release(); }

        // Remove the evicted article's data from Redis
        if (evicted.HasValue)
            await _cache.RemoveAsync(CacheKey(evicted.Value));

        await _cache.SetStringAsync(CacheKey(articleId), JsonSerializer.Serialize(comments));
    }

    public async Task InvalidateAsync(Guid articleId)
    {
        await _lock.WaitAsync();
        try
        {
            if (_lruMap.TryGetValue(articleId, out var node))
            {
                _lruList.Remove(node);
                _lruMap.Remove(articleId);
            }
        }
        finally { _lock.Release(); }

        await _cache.RemoveAsync(CacheKey(articleId));
    }

    // Move to front (most recently used) or add as new
    private void Touch(Guid articleId)
    {
        if (_lruMap.TryGetValue(articleId, out var node))
        {
            _lruList.Remove(node);
            _lruList.AddFirst(node);
        }
        else
        {
            var newNode = _lruList.AddFirst(articleId);
            _lruMap[articleId] = newNode;
        }
    }

    private static string CacheKey(Guid articleId) => $"comments:{articleId}";
}

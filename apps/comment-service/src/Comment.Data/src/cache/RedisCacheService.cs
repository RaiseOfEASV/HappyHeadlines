using System.Diagnostics.Metrics;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Comment.Data.cache;

public class RedisCacheService(IDistributedCache cache, IConnectionMultiplexer redis) : ICacheService
{
    private static readonly Meter Meter = new("happyheadlines.comment-service");
    private static readonly Counter<long> CacheHits = Meter.CreateCounter<long>("cache_hits", description: "Number of cache hits");
    private static readonly Counter<long> CacheMisses = Meter.CreateCounter<long>("cache_misses", description: "Number of cache misses");

    private const int MaxItems = 30;
    private const string LruSetKey = "cache:lru_index";

    private IDatabase Db => redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await cache.GetStringAsync(key);
        if (data is null)
        {
            CacheMisses.Add(1);
            return default;
        }

        CacheHits.Add(1);
        await Db.SortedSetAddAsync(LruSetKey, key, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var isNewKey = await Db.SortedSetScoreAsync(LruSetKey, key) is null;
        if (isNewKey)
            await EvictIfFullAsync();

        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiry;

        await cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
        await Db.SortedSetAddAsync(LruSetKey, key, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
    }

    public async Task RemoveAsync(string key)
    {
        await cache.RemoveAsync(key);
        await Db.SortedSetRemoveAsync(LruSetKey, key);
    }

    private async Task EvictIfFullAsync()
    {
        var count = await Db.SortedSetLengthAsync(LruSetKey);
        if (count < MaxItems) return;

        var oldest = await Db.SortedSetRangeByRankAsync(LruSetKey, 0, 0);
        if (oldest.Length == 0) return;

        var oldestKey = oldest[0].ToString();
        await cache.RemoveAsync(oldestKey);
        await Db.SortedSetRemoveAsync(LruSetKey, oldestKey);
    }
}
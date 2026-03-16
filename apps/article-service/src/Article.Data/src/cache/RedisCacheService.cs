using System.Diagnostics.Metrics;
using System.Text.Json;
using Article.Services.application_interfaces.ports;
using Microsoft.Extensions.Caching.Distributed;

namespace Article.Data.cache;

public class RedisCacheService(IDistributedCache cache) : ICacheService
{
    private static readonly Meter Meter = new("happyheadlines.article-service");
    private static readonly Counter<long> CacheHits = Meter.CreateCounter<long>("cache_hits", description: "Number of cache hits");
    private static readonly Counter<long> CacheMisses = Meter.CreateCounter<long>("cache_misses", description: "Number of cache misses");

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await cache.GetStringAsync(key);
        if (data is null)
        {
            CacheMisses.Add(1);
            return default;
        }
        CacheHits.Add(1);
        return JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue)
            options.AbsoluteExpirationRelativeToNow = expiry;

        await cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
    }

    public Task RemoveAsync(string key) => cache.RemoveAsync(key);
}

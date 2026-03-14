using System.Text.Json;
using Article.Services.application_interfaces.ports;
using Microsoft.Extensions.Caching.Distributed;

namespace Article.Data.cache;

public class RedisCacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await cache.GetStringAsync(key);
        return data is null ? default : JsonSerializer.Deserialize<T>(data);
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

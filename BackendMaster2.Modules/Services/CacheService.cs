using BackendMaster2.Modules.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace BackendMaster2.Modules.Services;

/// <summary>
/// Implementación del servicio de caché sobre IDistributedCache (Redis).
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    // TTL por defecto para maestros. Centralizado para poder ajustarlo en un solo sitio.
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(60);

    public CacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var cachedData = await _cache.GetStringAsync(key);

        if (string.IsNullOrEmpty(cachedData))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(cachedData);
    }

    public async Task SetAsync<T>(string key, T data, TimeSpan? expiration = null)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration
        };

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), cacheOptions);
    }

    public async Task RemoveAsync(params string[] keys)
    {
        foreach (var key in keys)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace CachingLab.Services;

public class CacheInvalidationService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;

    public CacheInvalidationService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }

    public async Task InvalidateProductAsync(int productId)
    {
        _memoryCache.Remove(CacheKeys.MemoryAllProducts);
        _memoryCache.Remove(CacheKeys.MemoryProductById(productId));

        await _distributedCache.RemoveAsync(CacheKeys.RedisAllProducts);
        await _distributedCache.RemoveAsync(CacheKeys.RedisProductById(productId));
    }
}
namespace CachingLab.Services;

public static class CacheKeys
{
    public const string MemoryAllProducts = "memory:products:all";

    public static string MemoryProductById(int id)
    {
        return $"memory:products:{id}";
    }

    public const string RedisAllProducts = "redis:products:all";

    public static string RedisProductById(int id)
    {
        return $"redis:products:{id}";
    }
}
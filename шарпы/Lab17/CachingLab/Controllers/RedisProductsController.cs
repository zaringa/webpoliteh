using System.Diagnostics;
using System.Text.Json;
using CachingLab.Models;
using CachingLab.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace CachingLab.Controllers;

[ApiController]
[Route("api/redis-products")]
public class RedisProductsController : ControllerBase
{
    private readonly IDistributedCache _distributedCache;
    private readonly ProductRepository _repository;
    private readonly CacheInvalidationService _cacheInvalidation;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public RedisProductsController(
        IDistributedCache distributedCache,
        ProductRepository repository,
        CacheInvalidationService cacheInvalidation)
    {
        _distributedCache = distributedCache;
        _repository = repository;
        _cacheInvalidation = cacheInvalidation;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stopwatch = Stopwatch.StartNew();

        var result = await GetOrSetAsync(
            CacheKeys.RedisAllProducts,
            () => _repository.GetAllSlowAsync());

        stopwatch.Stop();

        return Ok(new
        {
            source = result.CacheHit ? "redis distributed cache" : "database",
            elapsedMs = stopwatch.ElapsedMilliseconds,
            databaseHits = _repository.DatabaseHits,
            data = result.Value
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var stopwatch = Stopwatch.StartNew();

        var result = await GetOrSetAsync(
            CacheKeys.RedisProductById(id),
            () => _repository.GetByIdSlowAsync(id));

        stopwatch.Stop();

        if (result.Value is null)
        {
            return NotFound(new
            {
                message = $"Товар с id={id} не найден"
            });
        }

        return Ok(new
        {
            source = result.CacheHit ? "redis distributed cache" : "database",
            elapsedMs = stopwatch.ElapsedMilliseconds,
            databaseHits = _repository.DatabaseHits,
            data = result.Value
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductRequest request)
    {
        var product = await _repository.CreateAsync(request);

        await _cacheInvalidation.InvalidateProductAsync(product.Id);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductRequest request)
    {
        var product = await _repository.UpdateAsync(id, request);

        if (product is null)
        {
            return NotFound(new
            {
                message = $"Товар с id={id} не найден"
            });
        }

        await _cacheInvalidation.InvalidateProductAsync(id);

        return Ok(new
        {
            message = "Товар изменён, связанные записи удалены из Redis и memory cache",
            data = product
        });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Товар с id={id} не найден"
            });
        }

        await _cacheInvalidation.InvalidateProductAsync(id);

        return Ok(new
        {
            message = "Товар удалён, связанные записи удалены из Redis и memory cache"
        });
    }

    private async Task<(T? Value, bool CacheHit)> GetOrSetAsync<T>(
        string key,
        Func<Task<T?>> factory)
    {
        var cachedJson = await _distributedCache.GetStringAsync(key);

        if (!string.IsNullOrEmpty(cachedJson))
        {
            var cachedValue = JsonSerializer.Deserialize<T>(cachedJson, JsonOptions);
            return (cachedValue, true);
        }

        var value = await factory();

        if (value is not null)
        {
            var json = JsonSerializer.Serialize(value, JsonOptions);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(20),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            };

            await _distributedCache.SetStringAsync(key, json, cacheOptions);
        }

        return (value, false);
    }
}
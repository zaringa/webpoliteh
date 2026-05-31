using System.Diagnostics;
using CachingLab.Models;
using CachingLab.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace CachingLab.Controllers;

[ApiController]
[Route("api/memory-products")]
public class MemoryProductsController : ControllerBase
{
    private readonly IMemoryCache _memoryCache;
    private readonly ProductRepository _repository;
    private readonly CacheInvalidationService _cacheInvalidation;

    public MemoryProductsController(
        IMemoryCache memoryCache,
        ProductRepository repository,
        CacheInvalidationService cacheInvalidation)
    {
        _memoryCache = memoryCache;
        _repository = repository;
        _cacheInvalidation = cacheInvalidation;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var stopwatch = Stopwatch.StartNew();

        var cacheHit = true;

        if (!_memoryCache.TryGetValue(CacheKeys.MemoryAllProducts, out List<Product>? products))
        {
            cacheHit = false;

            products = await _repository.GetAllSlowAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(20))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

            _memoryCache.Set(CacheKeys.MemoryAllProducts, products, cacheOptions);
        }

        stopwatch.Stop();

        return Ok(new
        {
            source = cacheHit ? "memory cache" : "database",
            elapsedMs = stopwatch.ElapsedMilliseconds,
            databaseHits = _repository.DatabaseHits,
            data = products
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var stopwatch = Stopwatch.StartNew();

        var cacheKey = CacheKeys.MemoryProductById(id);
        var cacheHit = true;

        if (!_memoryCache.TryGetValue(cacheKey, out Product? product))
        {
            cacheHit = false;

            product = await _repository.GetByIdSlowAsync(id);

            if (product is null)
            {
                return NotFound(new
                {
                    message = $"Товар с id={id} не найден"
                });
            }

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(20))
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(60));

            _memoryCache.Set(cacheKey, product, cacheOptions);
        }

        stopwatch.Stop();

        return Ok(new
        {
            source = cacheHit ? "memory cache" : "database",
            elapsedMs = stopwatch.ElapsedMilliseconds,
            databaseHits = _repository.DatabaseHits,
            data = product
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
            message = "Товар изменён, связанные записи удалены из кэша",
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
            message = "Товар удалён, связанные записи удалены из кэша"
        });
    }
}
using System.Collections.Concurrent;
using CachingLab.Models;

namespace CachingLab.Services;

public class ProductRepository
{
    private readonly ConcurrentDictionary<int, Product> _products = new();
    private int _nextId = 2;
    private int _databaseHits;

    public ProductRepository()
    {
        _products[1] = new Product
        {
            Id = 1,
            Name = "Ноутбук",
            Price = 1200,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _products[2] = new Product
        {
            Id = 2,
            Name = "Смартфон",
            Price = 800,
            UpdatedAtUtc = DateTime.UtcNow
        };
    }

    public int DatabaseHits => _databaseHits;

    public async Task<List<Product>> GetAllSlowAsync()
    {
        await SimulateDatabaseDelayAsync();

        return _products.Values
            .OrderBy(p => p.Id)
            .Select(Clone)
            .ToList();
    }

    public async Task<Product?> GetByIdSlowAsync(int id)
    {
        await SimulateDatabaseDelayAsync();

        return _products.TryGetValue(id, out var product)
            ? Clone(product)
            : null;
    }

    public Task<Product> CreateAsync(ProductRequest request)
    {
        var id = Interlocked.Increment(ref _nextId);

        var product = new Product
        {
            Id = id,
            Name = string.IsNullOrWhiteSpace(request.Name) ? "Без названия" : request.Name.Trim(),
            Price = request.Price,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _products[id] = product;

        return Task.FromResult(Clone(product));
    }

    public Task<Product?> UpdateAsync(int id, ProductRequest request)
    {
        if (!_products.ContainsKey(id))
        {
            return Task.FromResult<Product?>(null);
        }

        var updated = new Product
        {
            Id = id,
            Name = string.IsNullOrWhiteSpace(request.Name) ? "Без названия" : request.Name.Trim(),
            Price = request.Price,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _products[id] = updated;

        return Task.FromResult<Product?>(Clone(updated));
    }

    public Task<bool> DeleteAsync(int id)
    {
        return Task.FromResult(_products.TryRemove(id, out _));
    }

    private async Task SimulateDatabaseDelayAsync()
    {
        Interlocked.Increment(ref _databaseHits);

        await Task.Delay(1200);
    }

    private static Product Clone(Product product)
    {
        return new Product
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            UpdatedAtUtc = product.UpdatedAtUtc
        };
    }
}
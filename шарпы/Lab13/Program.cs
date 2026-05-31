var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var catalog = new List<CatalogItem>
{
    new(1, "Mechanical Keyboard", "Peripherals", 5900m, 12, 4.8m),
    new(2, "Wireless Mouse", "Peripherals", 2400m, 18, 4.5m),
    new(3, "27\" Monitor", "Displays", 21990m, 6, 4.7m),
    new(4, "USB-C Hub", "Accessories", 3200m, 20, 4.3m),
    new(5, "NVMe SSD 1TB", "Storage", 8700m, 14, 4.9m),
    new(6, "External HDD 2TB", "Storage", 6100m, 9, 4.2m),
    new(7, "Laptop Stand", "Accessories", 1900m, 25, 4.4m),
    new(8, "24\" Monitor", "Displays", 17400m, 7, 4.6m)
};

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/catalog", (
    string? search,
    string? category,
    decimal? minPrice,
    decimal? maxPrice) =>
{
    var query = catalog.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        var term = search.Trim().ToLowerInvariant();
        query = query.Where(item =>
            item.Name.ToLowerInvariant().Contains(term) ||
            item.Category.ToLowerInvariant().Contains(term));
    }

    if (!string.IsNullOrWhiteSpace(category))
    {
        query = query.Where(item =>
            item.Category.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    if (minPrice is not null)
    {
        query = query.Where(item => item.Price >= minPrice);
    }

    if (maxPrice is not null)
    {
        query = query.Where(item => item.Price <= maxPrice);
    }

    var items = query.OrderBy(item => item.Name).ToArray();

    return Results.Ok(new
    {
        total = items.Length,
        items
    });
});

app.MapGet("/api/catalog/summary", (
    string? search,
    string? category,
    decimal? minPrice,
    decimal? maxPrice) =>
{
    var query = catalog.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        var term = search.Trim().ToLowerInvariant();
        query = query.Where(item =>
            item.Name.ToLowerInvariant().Contains(term) ||
            item.Category.ToLowerInvariant().Contains(term));
    }

    if (!string.IsNullOrWhiteSpace(category))
    {
        query = query.Where(item =>
            item.Category.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    if (minPrice is not null)
    {
        query = query.Where(item => item.Price >= minPrice);
    }

    if (maxPrice is not null)
    {
        query = query.Where(item => item.Price <= maxPrice);
    }

    var items = query.ToArray();
    var categories = items
        .GroupBy(item => item.Category)
        .OrderBy(group => group.Key)
        .Select(group => new
        {
            category = group.Key,
            count = group.Count(),
            averagePrice = Math.Round(group.Average(item => item.Price), 2)
        })
        .ToArray();

    return Results.Ok(new
    {
        count = items.Length,
        totalInventoryUnits = items.Sum(item => item.Quantity),
        averageRating = items.Length == 0 ? 0 : Math.Round(items.Average(item => item.Rating), 2),
        inventoryValue = items.Sum(item => item.Price * item.Quantity),
        categories
    });
});

app.MapGet("/api/catalog/categories", () =>
{
    var categories = catalog
        .Select(item => item.Category)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(categoryName => categoryName)
        .ToArray();

    return Results.Ok(categories);
});

app.MapGet("/api/catalog/error-demo", () =>
{
    throw new InvalidOperationException("Demo backend error from /api/catalog/error-demo");
});

app.MapFallbackToFile("index.html");

app.Run();

internal sealed record CatalogItem(
    int Id,
    string Name,
    string Category,
    decimal Price,
    int Quantity,
    decimal Rating);

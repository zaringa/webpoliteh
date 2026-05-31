// Явно добавляем недостающие пространства имён
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/products", (string? category, decimal? minPrice) =>
{
    var products = new List<Product>
    {
        new Product(1, "Laptop", 1200, "Electronics"),
        new Product(2, "Mouse", 25, "Electronics"),
        new Product(3, "Book", 15, "Books"),
        new Product(4, "Pen", 2, "Stationery")
    };

    var filtered = products.AsEnumerable();
    if (!string.IsNullOrEmpty(category))
        filtered = filtered.Where(p => p.Category == category);
    if (minPrice.HasValue)
        filtered = filtered.Where(p => p.Price >= minPrice);

    return Results.Ok(filtered);
})
.WithName("GetProducts")
.WithOpenApi();

app.MapPost("/products", (Product newProduct) =>
{
    var createdProduct = newProduct with { Id = new Random().Next(100, 999) };
    return Results.Created($"/products/{createdProduct.Id}", createdProduct);
})
.WithName("CreateProduct")
.WithOpenApi();

app.MapPost("/products/category", (string category, Product product) =>
{
    var newProduct = product with { Category = category };
    return Results.Ok(newProduct);
})
.WithName("CreateProductWithCategory")
.WithOpenApi();

app.MapPut("/products/{id}", (int id, Product updatedProduct) =>
{
    return Results.Ok(updatedProduct with { Id = id });
})
.WithName("UpdateProduct")
.WithOpenApi();

app.MapPatch("/products/{id}", (int id, Dictionary<string, object> updates) =>
{
    var product = new Product(id, "Sample", 100, "Temp");

    if (updates.TryGetValue("name", out var name))
        product = product with { Name = name.ToString() };
    if (updates.TryGetValue("price", out var price))
        product = product with { Price = Convert.ToDecimal(price) };
    if (updates.TryGetValue("category", out var category))
        product = product with { Category = category.ToString() };

    return Results.Ok(product);
})
.WithName("PatchProduct")
.WithOpenApi();

app.Run();

public record Product(int Id, string Name, decimal Price, string? Category = null);
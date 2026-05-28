using Lab12.Data;
using Lab12.Dtos;
using Lab12.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Lab12.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProductsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductReadDto>>> GetAll()
    {
        var products = await _db.Products
            .OrderBy(product => product.Id)
            .Select(MapToReadDto())
            .ToListAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductReadDto>> GetById(int id)
    {
        var product = await _db.Products
            .Where(item => item.Id == id)
            .Select(MapToReadDto())
            .FirstOrDefaultAsync();

        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductReadDto>> Create([FromBody] ProductUpsertDto dto)
    {
        ValidateBusinessRules(dto);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var product = new Product
        {
            Name = dto.Name,
            Sku = dto.Sku,
            Price = dto.Price,
            DiscountPrice = dto.DiscountPrice,
            StockQuantity = dto.StockQuantity
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        var result = await _db.Products
            .Where(item => item.Id == product.Id)
            .Select(MapToReadDto())
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpsertDto dto)
    {
        ValidateBusinessRules(dto);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var product = await _db.Products.FindAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        product.Name = dto.Name;
        product.Sku = dto.Sku;
        product.Price = dto.Price;
        product.DiscountPrice = dto.DiscountPrice;
        product.StockQuantity = dto.StockQuantity;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private void ValidateBusinessRules(ProductUpsertDto dto)
    {
        if (dto.DiscountPrice is not null && dto.DiscountPrice >= dto.Price)
        {
            ModelState.AddModelError(
                nameof(dto.DiscountPrice),
                "DiscountPrice must be less than Price.");
        }

        if (dto.Sku.StartsWith("VIP-", StringComparison.OrdinalIgnoreCase) && dto.Price < 100)
        {
            ModelState.AddModelError(
                nameof(dto.Price),
                "Products with SKU prefix VIP- must have price >= 100.");
        }
    }

    private static Expression<Func<Product, ProductReadDto>> MapToReadDto()
    {
        return product => new ProductReadDto(
            product.Id,
            product.Name,
            product.Sku,
            product.Price,
            product.DiscountPrice,
            product.StockQuantity);
    }
}

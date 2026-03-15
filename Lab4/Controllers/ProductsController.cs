using Lab4.Dtos;
using Lab4.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> Products =
    [
        new Product
        {
            Id = 1,
            Name = "Laptop",
            Description = "Ultrabook for study and development",
            Price = 1199.99m,
            CreatedAt = new DateTime(2026, 3, 1, 10, 0, 0, DateTimeKind.Utc)
        },
        new Product
        {
            Id = 2,
            Name = "Mouse",
            Description = "Wireless optical mouse",
            Price = 35.50m,
            CreatedAt = new DateTime(2026, 3, 5, 14, 30, 0, DateTimeKind.Utc)
        }
    ];

    private static int _nextId = Products.Max(p => p.Id) + 1;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        return Ok(Products);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetById(int id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product is null)
        {
            return NotFound(new { message = $"Product with id={id} not found." });
        }

        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Product> Create([FromBody] CreateProductDto dto)
    {
        var product = new Product
        {
            Id = _nextId++,
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim(),
            Price = dto.Price,
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow
        };

        Products.Add(product);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] UpdateProductDto dto)
    {
        var existing = Products.FirstOrDefault(p => p.Id == id);
        if (existing is null)
        {
            return NotFound(new { message = $"Product with id={id} not found." });
        }

        existing.Name = dto.Name.Trim();
        existing.Description = dto.Description.Trim();
        existing.Price = dto.Price;
        existing.CreatedAt = dto.CreatedAt ?? existing.CreatedAt;

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var existing = Products.FirstOrDefault(p => p.Id == id);
        if (existing is null)
        {
            return NotFound(new { message = $"Product with id={id} not found." });
        }

        Products.Remove(existing);
        return NoContent();
    }
}

using Lab5.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> Products =
    [
        new Product
        {
            Id = 1,
            ExternalId = Guid.Parse("3ca05388-6eb7-449c-bf2f-cf338f9a5de8"),
            Name = "Laptop",
            Slug = "laptop-pro",
            Price = 1200.00m,
            CreatedAt = new DateTime(2024, 10, 1, 9, 0, 0, DateTimeKind.Utc)
        },
        new Product
        {
            Id = 2,
            ExternalId = Guid.Parse("1a647e16-1940-4e9b-a082-4f8f8f1d8d45"),
            Name = "Mouse",
            Slug = "mouse-wireless",
            Price = 35.50m,
            CreatedAt = new DateTime(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc)
        },
        new Product
        {
            Id = 3,
            ExternalId = Guid.Parse("8e700804-4adc-4ae4-b298-09d6397048db"),
            Name = "Keyboard",
            Slug = "keyboard-rgb",
            Price = 90.00m,
            CreatedAt = new DateTime(2026, 3, 1, 8, 30, 0, DateTimeKind.Utc)
        }
    ];

    private static int _nextId = Products.Max(product => product.Id) + 1;

    [HttpGet]
    public ActionResult<object> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sort = "id")
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("page и pageSize должны быть больше 0.");
        }

        var query = ApplySort(Products.AsQueryable(), sort);
        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new
        {
            page,
            pageSize,
            total = Products.Count,
            sort,
            items
        });
    }

    [HttpGet("{id:int}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = Products.FirstOrDefault(item => item.Id == id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> Create([FromBody] ProductCreateRequest request)
    {
        if (!IsValidInput(request.Name, request.Slug))
        {
            return BadRequest("Name и Slug обязательны. Slug должен содержать минимум 3 символа.");
        }

        var product = new Product
        {
            Id = _nextId++,
            ExternalId = request.ExternalId ?? Guid.NewGuid(),
            Name = request.Name.Trim(),
            Slug = request.Slug.Trim().ToLowerInvariant(),
            Price = request.Price,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow
        };

        Products.Add(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Product> Update(int id, [FromBody] ProductUpdateRequest request)
    {
        if (!IsValidInput(request.Name, request.Slug))
        {
            return BadRequest("Name и Slug обязательны. Slug должен содержать минимум 3 символа.");
        }

        var existing = Products.FirstOrDefault(item => item.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = request.Name.Trim();
        existing.Slug = request.Slug.Trim().ToLowerInvariant();
        existing.Price = request.Price;
        existing.CreatedAt = request.CreatedAt ?? existing.CreatedAt;
        existing.ExternalId = request.ExternalId ?? existing.ExternalId;

        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = Products.FirstOrDefault(item => item.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        Products.Remove(existing);
        return NoContent();
    }

    [HttpGet("by-name/{name}")]
    public ActionResult<IEnumerable<Product>> GetByName(string name)
    {
        var matched = Products
            .Where(item => item.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(matched);
    }

    [HttpGet("by-date/{date:datetime}")]
    public ActionResult<IEnumerable<Product>> GetByDate(DateTime date)
    {
        var matched = Products
            .Where(item => item.CreatedAt.Date == date.Date)
            .ToList();

        return Ok(matched);
    }

    [HttpGet("by-guid/{guid:guid}")]
    public ActionResult<Product> GetByGuid(Guid guid)
    {
        var product = Products.FirstOrDefault(item => item.ExternalId == guid);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("by-slug/{slug:minlength(3)}")]
    public ActionResult<Product> GetBySlug(string slug)
    {
        var product = Products.FirstOrDefault(item =>
            item.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("details/{id:int?}")]
    public ActionResult<object> GetDetails(int? id = null)
    {
        if (id is null)
        {
            return Ok(new
            {
                message = "Параметр id не передан, возвращается краткий список товаров.",
                items = Products.Select(item => new { item.Id, item.Name, item.Price })
            });
        }

        var product = Products.FirstOrDefault(item => item.Id == id.Value);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("archive/{year:int=2026}")]
    public ActionResult<IEnumerable<Product>> GetByYear(int year = 2026)
    {
        var matched = Products
            .Where(item => item.CreatedAt.Year == year)
            .ToList();

        return Ok(matched);
    }

    private static IQueryable<Product> ApplySort(IQueryable<Product> query, string sort)
    {
        return sort.ToLowerInvariant() switch
        {
            "name" => query.OrderBy(item => item.Name),
            "price" => query.OrderBy(item => item.Price),
            "createdat" => query.OrderBy(item => item.CreatedAt),
            _ => query.OrderBy(item => item.Id)
        };
    }

    private static bool IsValidInput(string name, string slug)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               !string.IsNullOrWhiteSpace(slug) &&
               slug.Trim().Length >= 3;
    }
}

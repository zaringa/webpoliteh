using Lab11.Data;
using Lab11.Dtos;
using Lab11.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab11.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CategoryReadDto>>> GetAll()
    {
        var categories = await _db.Categories
            .OrderBy(category => category.Id)
            .Select(category => new CategoryReadDto(
                category.Id,
                category.Name,
                category.Description,
                category.Books.Count))
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CategoryReadDto>> GetById(int id)
    {
        var category = await _db.Categories
            .Where(item => item.Id == id)
            .Select(item => new CategoryReadDto(
                item.Id,
                item.Name,
                item.Description,
                item.Books.Count))
            .FirstOrDefaultAsync();

        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryReadDto>> Create([FromBody] CategoryUpsertDto dto)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        _db.Categories.Add(category);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Category with the same name already exists."
            });
        }

        var result = new CategoryReadDto(category.Id, category.Name, category.Description, 0);

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpsertDto dto)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        category.Name = dto.Name;
        category.Description = dto.Description;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "Category with the same name already exists."
            });
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        var hasBooks = await _db.Books.AnyAsync(book => book.CategoryId == id);
        if (hasBooks)
        {
            return Conflict(new
            {
                message = "Cannot delete category because related books exist."
            });
        }

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

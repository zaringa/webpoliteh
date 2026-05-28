using Lab11.Data;
using Lab11.Dtos;
using Lab11.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab11.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthorsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<AuthorReadDto>>> GetAll()
    {
        var authors = await _db.Authors
            .OrderBy(author => author.Id)
            .Select(author => new AuthorReadDto(
                author.Id,
                author.Name,
                author.Email,
                author.Books.Count))
            .ToListAsync();

        return Ok(authors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorReadDto>> GetById(int id)
    {
        var author = await _db.Authors
            .Where(item => item.Id == id)
            .Select(item => new AuthorReadDto(
                item.Id,
                item.Name,
                item.Email,
                item.Books.Count))
            .FirstOrDefaultAsync();

        return author is null ? NotFound() : Ok(author);
    }

    [HttpPost]
    public async Task<ActionResult<AuthorReadDto>> Create([FromBody] AuthorUpsertDto dto)
    {
        var author = new Author
        {
            Name = dto.Name,
            Email = dto.Email
        };

        _db.Authors.Add(author);
        await _db.SaveChangesAsync();

        var result = new AuthorReadDto(author.Id, author.Name, author.Email, 0);

        return CreatedAtAction(nameof(GetById), new { id = author.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] AuthorUpsertDto dto)
    {
        var author = await _db.Authors.FindAsync(id);
        if (author is null)
        {
            return NotFound();
        }

        author.Name = dto.Name;
        author.Email = dto.Email;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _db.Authors.FindAsync(id);
        if (author is null)
        {
            return NotFound();
        }

        var hasBooks = await _db.Books.AnyAsync(book => book.AuthorId == id);
        if (hasBooks)
        {
            return Conflict(new
            {
                message = "Cannot delete author because related books exist."
            });
        }

        _db.Authors.Remove(author);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

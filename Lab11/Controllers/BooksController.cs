using Lab11.Data;
using Lab11.Dtos;
using Lab11.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab11.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _db;

    public BooksController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<BookReadDto>>> GetAll()
    {
        var books = await _db.Books
            .Include(book => book.Author)
            .Include(book => book.Category)
            .OrderBy(book => book.Id)
            .Select(book => new BookReadDto(
                book.Id,
                book.Title,
                book.Year,
                book.Price,
                book.AuthorId,
                book.Author!.Name,
                book.CategoryId,
                book.Category!.Name))
            .ToListAsync();

        return Ok(books);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookReadDto>> GetById(int id)
    {
        var book = await _db.Books
            .Include(item => item.Author)
            .Include(item => item.Category)
            .Where(item => item.Id == id)
            .Select(item => new BookReadDto(
                item.Id,
                item.Title,
                item.Year,
                item.Price,
                item.AuthorId,
                item.Author!.Name,
                item.CategoryId,
                item.Category!.Name))
            .FirstOrDefaultAsync();

        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookReadDto>> Create([FromBody] BookUpsertDto dto)
    {
        if (!await _db.Authors.AnyAsync(author => author.Id == dto.AuthorId))
        {
            return BadRequest(new { message = "Author does not exist." });
        }

        if (!await _db.Categories.AnyAsync(category => category.Id == dto.CategoryId))
        {
            return BadRequest(new { message = "Category does not exist." });
        }

        var book = new Book
        {
            Title = dto.Title,
            Year = dto.Year,
            Price = dto.Price,
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId
        };

        _db.Books.Add(book);
        await _db.SaveChangesAsync();

        var createdBook = await _db.Books
            .Include(item => item.Author)
            .Include(item => item.Category)
            .Where(item => item.Id == book.Id)
            .Select(item => new BookReadDto(
                item.Id,
                item.Title,
                item.Year,
                item.Price,
                item.AuthorId,
                item.Author!.Name,
                item.CategoryId,
                item.Category!.Name))
            .FirstAsync();

        return CreatedAtAction(nameof(GetById), new { id = book.Id }, createdBook);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] BookUpsertDto dto)
    {
        var book = await _db.Books.FindAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        if (!await _db.Authors.AnyAsync(author => author.Id == dto.AuthorId))
        {
            return BadRequest(new { message = "Author does not exist." });
        }

        if (!await _db.Categories.AnyAsync(category => category.Id == dto.CategoryId))
        {
            return BadRequest(new { message = "Category does not exist." });
        }

        book.Title = dto.Title;
        book.Year = dto.Year;
        book.Price = dto.Price;
        book.AuthorId = dto.AuthorId;
        book.CategoryId = dto.CategoryId;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        _db.Books.Remove(book);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}

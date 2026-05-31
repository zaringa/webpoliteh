using Lab6.Dtos;
using Lab6.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab6.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _service;

    public BooksController(IBookService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<BookDto>> GetAll()
    {
        return Ok(_service.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<BookDto> GetById(int id)
    {
        var book = _service.GetById(id);
        return book is null ? NotFound() : Ok(book);
    }

    [HttpPost]
    public ActionResult<BookDto> Create([FromBody] BookCreateDto dto)
    {
        var created = _service.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}

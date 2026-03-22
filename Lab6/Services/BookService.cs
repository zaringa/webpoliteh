using Lab6.Dtos;
using Lab6.Models;
using Lab6.Repositories;

namespace Lab6.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _repository;

    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyCollection<BookDto> GetAll()
    {
        return _repository.GetAll().Select(ToDto).ToList();
    }

    public BookDto? GetById(int id)
    {
        var book = _repository.GetById(id);
        return book is null ? null : ToDto(book);
    }

    public BookDto Create(BookCreateDto dto)
    {
        var book = new Book
        {
            Name = dto.Name.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        var created = _repository.Add(book);
        return ToDto(created);
    }

    private static BookDto ToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Name = book.Name,
            CreatedAt = book.CreatedAt
        };
    }
}

using Lab6.Dtos;

namespace Lab6.Services;

public interface IBookService
{
    IReadOnlyCollection<BookDto> GetAll();
    BookDto? GetById(int id);
    BookDto Create(BookCreateDto dto);
}

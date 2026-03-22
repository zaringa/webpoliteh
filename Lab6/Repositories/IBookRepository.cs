using Lab6.Models;

namespace Lab6.Repositories;

public interface IBookRepository
{
    IReadOnlyCollection<Book> GetAll();
    Book? GetById(int id);
    Book Add(Book book);
}

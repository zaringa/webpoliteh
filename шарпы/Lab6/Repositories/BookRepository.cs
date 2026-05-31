using Lab6.Models;

namespace Lab6.Repositories;

public class BookRepository : IBookRepository
{
    private readonly Dictionary<int, Book> _books = new();
    private int _nextId = 1;
    private readonly object _sync = new();

    public IReadOnlyCollection<Book> GetAll()
    {
        lock (_sync)
        {
            return _books.Values
                .OrderBy(book => book.Id)
                .Select(Clone)
                .ToList();
        }
    }

    public Book? GetById(int id)
    {
        lock (_sync)
        {
            return _books.TryGetValue(id, out var book) ? Clone(book) : null;
        }
    }

    public Book Add(Book book)
    {
        lock (_sync)
        {
            var copy = Clone(book);
            copy.Id = _nextId++;
            _books[copy.Id] = copy;
            return Clone(copy);
        }
    }

    private static Book Clone(Book source)
    {
        return new Book
        {
            Id = source.Id,
            Name = source.Name,
            CreatedAt = source.CreatedAt
        };
    }
}

using System.ComponentModel.DataAnnotations;

namespace Lab11.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(0, 100000)]
    public decimal Price { get; set; }

    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}

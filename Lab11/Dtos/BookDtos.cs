using System.ComponentModel.DataAnnotations;

namespace Lab11.Dtos;

public record BookReadDto(
    int Id,
    string Title,
    int Year,
    decimal Price,
    int AuthorId,
    string AuthorName,
    int CategoryId,
    string CategoryName);

public class BookUpsertDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(0, 100000)]
    public decimal Price { get; set; }

    public int AuthorId { get; set; }
    public int CategoryId { get; set; }
}

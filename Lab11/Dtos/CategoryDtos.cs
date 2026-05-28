using System.ComponentModel.DataAnnotations;

namespace Lab11.Dtos;

public record CategoryReadDto(int Id, string Name, string? Description, int BooksCount);

public class CategoryUpsertDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }
}

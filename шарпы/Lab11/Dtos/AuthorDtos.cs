using System.ComponentModel.DataAnnotations;

namespace Lab11.Dtos;

public record AuthorReadDto(int Id, string Name, string? Email, int BooksCount);

public class AuthorUpsertDto
{
    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Email { get; set; }
}

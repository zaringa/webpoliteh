using System.ComponentModel.DataAnnotations;

namespace Lab6.Dtos;

public class BookCreateDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace Lab11.Models;

public class Author
{
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Email { get; set; }

    public List<Book> Books { get; set; } = [];
}

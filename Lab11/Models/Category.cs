using System.ComponentModel.DataAnnotations;

namespace Lab11.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Description { get; set; }

    public List<Book> Books { get; set; } = [];
}

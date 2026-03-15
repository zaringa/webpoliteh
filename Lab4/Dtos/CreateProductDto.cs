using System.ComponentModel.DataAnnotations;

namespace Lab4.Dtos;

public class CreateProductDto
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name length must be between 2 and 100.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "Description length must be between 5 and 500.")]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, 1_000_000, ErrorMessage = "Price must be between 0.01 and 1000000.")]
    public decimal Price { get; set; }

    public DateTime? CreatedAt { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Lab12.Dtos;

public record ProductReadDto(
    int Id,
    string Name,
    string Sku,
    decimal Price,
    decimal? DiscountPrice,
    int StockQuantity);

public class ProductUpsertDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string Sku { get; set; } = string.Empty;

    [Range(0.01, 100000)]
    public decimal Price { get; set; }

    [Range(0, 100000)]
    public decimal? DiscountPrice { get; set; }

    [Range(0, 100000)]
    public int StockQuantity { get; set; }
}

namespace Lab5.Models;

public class Product
{
    public int Id { get; set; }
    public Guid ExternalId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid? ExternalId { get; set; }
}

public class ProductUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime? CreatedAt { get; set; }
    public Guid? ExternalId { get; set; }
}

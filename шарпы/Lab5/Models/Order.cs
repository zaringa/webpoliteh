namespace Lab5.Models;

public class Order
{
    public int Id { get; set; }
    public Guid TrackingId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class OrderCreateRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public DateTime? OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "new";
    public Guid? TrackingId { get; set; }
}

public class OrderUpdateRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public DateTime? OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "new";
    public Guid? TrackingId { get; set; }
}

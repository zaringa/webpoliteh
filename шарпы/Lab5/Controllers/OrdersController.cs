using Lab5.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private static readonly List<Order> Orders =
    [
        new Order
        {
            Id = 1,
            TrackingId = Guid.Parse("66f8f702-c6c5-4af1-b4fd-c37d2fa254d2"),
            CustomerName = "Alex Ivanov",
            OrderDate = new DateTime(2025, 12, 10, 8, 0, 0, DateTimeKind.Utc),
            TotalAmount = 250.00m,
            Status = "new"
        },
        new Order
        {
            Id = 2,
            TrackingId = Guid.Parse("4c875bc2-9d26-4a12-8bbf-c1bd094f8f19"),
            CustomerName = "Maria Sokolova",
            OrderDate = new DateTime(2026, 2, 5, 11, 30, 0, DateTimeKind.Utc),
            TotalAmount = 780.00m,
            Status = "paid"
        }
    ];

    private static int _nextId = Orders.Max(order => order.Id) + 1;

    [HttpGet]
    public ActionResult<object> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sort = "id",
        [FromQuery] string? status = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("page и pageSize должны быть больше 0.");
        }

        var query = Orders.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(order =>
                order.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        query = sort.ToLowerInvariant() switch
        {
            "customer" => query.OrderBy(order => order.CustomerName),
            "date" => query.OrderBy(order => order.OrderDate),
            "amount" => query.OrderBy(order => order.TotalAmount),
            _ => query.OrderBy(order => order.Id)
        };

        var total = query.Count();
        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new
        {
            page,
            pageSize,
            total,
            sort,
            status,
            items
        });
    }

    [HttpGet("{id:int}")]
    public ActionResult<Order> GetById(int id)
    {
        var order = Orders.FirstOrDefault(item => item.Id == id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public ActionResult<Order> Create([FromBody] OrderCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName) || request.TotalAmount < 0)
        {
            return BadRequest("CustomerName обязателен, TotalAmount должен быть >= 0.");
        }

        var order = new Order
        {
            Id = _nextId++,
            TrackingId = request.TrackingId ?? Guid.NewGuid(),
            CustomerName = request.CustomerName.Trim(),
            OrderDate = request.OrderDate ?? DateTime.UtcNow,
            TotalAmount = request.TotalAmount,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "new" : request.Status.Trim()
        };

        Orders.Add(order);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Order> Update(int id, [FromBody] OrderUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName) || request.TotalAmount < 0)
        {
            return BadRequest("CustomerName обязателен, TotalAmount должен быть >= 0.");
        }

        var existing = Orders.FirstOrDefault(item => item.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.CustomerName = request.CustomerName.Trim();
        existing.OrderDate = request.OrderDate ?? existing.OrderDate;
        existing.TotalAmount = request.TotalAmount;
        existing.Status = string.IsNullOrWhiteSpace(request.Status) ? existing.Status : request.Status.Trim();
        existing.TrackingId = request.TrackingId ?? existing.TrackingId;

        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = Orders.FirstOrDefault(item => item.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        Orders.Remove(existing);
        return NoContent();
    }

    [HttpGet("by-customer/{name}")]
    public ActionResult<IEnumerable<Order>> GetByCustomerName(string name)
    {
        var matched = Orders
            .Where(order => order.CustomerName.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(matched);
    }

    [HttpGet("by-year/{year:int}")]
    public ActionResult<IEnumerable<Order>> GetByYear(int year)
    {
        var matched = Orders
            .Where(order => order.OrderDate.Year == year)
            .ToList();

        return Ok(matched);
    }

    [HttpGet("by-date/{date:datetime}")]
    public ActionResult<IEnumerable<Order>> GetByDate(DateTime date)
    {
        var matched = Orders
            .Where(order => order.OrderDate.Date == date.Date)
            .ToList();

        return Ok(matched);
    }

    [HttpGet("tracking/{guid:guid}")]
    public ActionResult<Order> GetByTrackingId(Guid guid)
    {
        var order = Orders.FirstOrDefault(item => item.TrackingId == guid);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("summary/{id:int?}")]
    public ActionResult<object> GetSummary(int? id = null)
    {
        if (id is null)
        {
            return Ok(new
            {
                totalOrders = Orders.Count,
                totalAmount = Orders.Sum(order => order.TotalAmount)
            });
        }

        var order = Orders.FirstOrDefault(item => item.Id == id.Value);
        return order is null ? NotFound() : Ok(order);
    }
}

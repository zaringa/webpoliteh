namespace Lab10.Services;

public sealed class OrderProcessingService
{
    private readonly ILogger<OrderProcessingService> _logger;

    public OrderProcessingService(ILogger<OrderProcessingService> logger)
    {
        _logger = logger;
    }

    public object Process(int orderId, bool fail)
    {
        _logger.LogDebug("Start processing order {OrderId}", orderId);

        if (orderId <= 0)
        {
            _logger.LogWarning("Invalid order id: {OrderId}", orderId);
            return new
            {
                success = false,
                error = "Order id must be greater than 0"
            };
        }

        try
        {
            if (fail)
            {
                throw new InvalidOperationException($"Artificial failure for order {orderId}");
            }

            _logger.LogInformation("Order {OrderId} processed successfully", orderId);

            return new
            {
                success = true,
                orderId,
                processedAt = DateTimeOffset.Now
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Order {OrderId} processing failed", orderId);
            return new
            {
                success = false,
                error = ex.Message,
                orderId
            };
        }
    }
}

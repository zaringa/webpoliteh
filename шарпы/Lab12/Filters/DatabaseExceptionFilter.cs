using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Lab12.Filters;

public class DatabaseExceptionFilter : IExceptionFilter
{
    private readonly ILogger<DatabaseExceptionFilter> _logger;

    public DatabaseExceptionFilter(ILogger<DatabaseExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DbUpdateConcurrencyException concurrencyException)
        {
            _logger.LogWarning(concurrencyException, "Database concurrency exception");

            context.Result = new ObjectResult(new ProblemDetails
            {
                Title = "Database concurrency error",
                Detail = "The data was changed by another request. Retry the operation.",
                Status = StatusCodes.Status409Conflict,
                Instance = context.HttpContext.Request.Path
            })
            {
                StatusCode = StatusCodes.Status409Conflict
            };
            context.ExceptionHandled = true;
            return;
        }

        if (context.Exception is not DbUpdateException dbUpdateException)
        {
            return;
        }

        _logger.LogError(dbUpdateException, "Database update exception");

        var detail = "Database update failed. Check unique values and foreign keys.";

        if (dbUpdateException.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true)
        {
            detail = "Duplicate value violates a unique constraint.";
        }

        context.Result = new ObjectResult(new ProblemDetails
        {
            Title = "Database error",
            Detail = detail,
            Status = StatusCodes.Status409Conflict,
            Instance = context.HttpContext.Request.Path
        })
        {
            StatusCode = StatusCodes.Status409Conflict
        };
        context.ExceptionHandled = true;
    }
}

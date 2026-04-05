using MyFirstWebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseMiddleware<EndpointTimingMiddleware>();
app.UseMiddleware<RequestTraceMiddleware>();
app.UseMiddleware<BlockPathMiddleware>();

app.MapGet("/ping", () => "pong");

app.MapGet("/trace", (HttpContext context) =>
{
    if (context.Items.TryGetValue("TraceId", out var traceId) && traceId is string traceIdValue)
    {
        return Results.Ok(traceIdValue);
    }

    return Results.Problem("TraceId is not available in HttpContext.Items.");
});

app.MapGet("/error", (HttpContext _) => throw new InvalidOperationException("Test exception from /error endpoint."));

app.Run();

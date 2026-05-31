namespace MyFirstWebApi.Middleware;

public sealed class RequestTraceMiddleware
{
    private readonly RequestDelegate _next;

    public RequestTraceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Guid.NewGuid().ToString();

        context.Items["TraceId"] = traceId;
        context.Response.Headers["X-Trace-Id"] = traceId;

        await _next(context);
    }
}

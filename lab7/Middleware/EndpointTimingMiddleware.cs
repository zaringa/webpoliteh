using System.Diagnostics;
using System.Globalization;

namespace MyFirstWebApi.Middleware;

public sealed class EndpointTimingMiddleware
{
    private readonly RequestDelegate _next;

    public EndpointTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        context.Response.OnStarting(static state =>
        {
            var (httpContext, sw) = ((HttpContext Context, Stopwatch Stopwatch))state;

            if (sw.IsRunning)
            {
                sw.Stop();
            }

            httpContext.Response.Headers["X-Endpoint-Elapsed-Ms"] =
                sw.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture);

            return Task.CompletedTask;
        }, (context, stopwatch));

        await _next(context);
    }
}

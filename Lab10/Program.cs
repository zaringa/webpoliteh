using Lab10.Logging;
using Lab10.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var fileLoggerOptions = builder.Configuration.GetSection("Logging:File").Get<FileLoggerOptions>() ?? new FileLoggerOptions();
var databaseLoggerOptions = builder.Configuration.GetSection("Logging:Database").Get<DatabaseLoggerOptions>() ?? new DatabaseLoggerOptions();

var fileLoggerProvider = new FileLoggerProvider(Options.Create(fileLoggerOptions), builder.Environment);
var databaseLogStore = new DatabaseLogStore(Options.Create(databaseLoggerOptions), builder.Environment);
var databaseLoggerProvider = new DatabaseLoggerProvider(Options.Create(databaseLoggerOptions), databaseLogStore);

builder.Services.AddSingleton(fileLoggerOptions);
builder.Services.AddSingleton(databaseLoggerOptions);
builder.Services.AddSingleton(databaseLogStore);
builder.Services.AddTransient<OrderProcessingService>();

builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
    options.SingleLine = true;
    options.IncludeScopes = true;
});
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
builder.Logging.AddProvider(fileLoggerProvider);
builder.Logging.AddProvider(databaseLoggerProvider);

var app = builder.Build();

var startupLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Lab10.Startup");
startupLogger.LogInformation("Application started in {Environment}", app.Environment.EnvironmentName);

app.MapGet("/", (ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("Lab10.Endpoints.Home");
    logger.LogInformation("Home endpoint requested");

    return Results.Ok(new
    {
        message = "Lab10 logging demo is running",
        endpoints = new[]
        {
            "/orders/{id}?fail=false",
            "/logs/generate?count=5&withError=false",
            "/logs/file?lines=20",
            "/logs/database?take=20",
            "/logs/providers"
        }
    });
});

app.MapPost("/orders/{id:int}", (
    int id,
    bool fail,
    OrderProcessingService service,
    ILoggerFactory loggerFactory) =>
{
    var endpointLogger = loggerFactory.CreateLogger("Lab10.Endpoints.OrderEndpoint");

    using var _ = endpointLogger.BeginScope("OrderRequest:{OrderId}", id);
    endpointLogger.LogInformation("Incoming request for order {OrderId}. fail={Fail}", id, fail);

    var result = service.Process(id, fail);
    return Results.Ok(result);
});

app.MapPost("/logs/generate", (int count, bool withError, ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("Lab10.Endpoints.LogGenerator");
    count = Math.Clamp(count, 1, 50);

    using var _ = logger.BeginScope("GenerateLogs");
    logger.LogInformation("Generating {Count} log batch entries", count);

    for (var i = 1; i <= count; i++)
    {
        logger.LogDebug("Debug record #{Index}", i);
        logger.LogInformation("Information record #{Index}", i);

        if (i % 2 == 0)
        {
            logger.LogWarning("Warning record #{Index}", i);
        }
    }

    if (withError)
    {
        try
        {
            throw new ApplicationException("Requested test exception");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error entry created by /logs/generate");
        }
    }

    return Results.Ok(new
    {
        generated = count,
        withError
    });
});

app.MapGet("/logs/file", (int lines, FileLoggerOptions fileOptions, IWebHostEnvironment env) =>
{
    lines = Math.Clamp(lines, 1, 200);

    var path = fileOptions.Path;
    if (!System.IO.Path.IsPathRooted(path))
    {
        path = System.IO.Path.Combine(env.ContentRootPath, path);
    }

    if (!File.Exists(path))
    {
        return Results.NotFound(new
        {
            message = "File log not found",
            path
        });
    }

    var content = File.ReadLines(path).TakeLast(lines).ToArray();

    return Results.Ok(new
    {
        path,
        lines = content
    });
});

app.MapGet("/logs/database", (int take, string? level, DatabaseLogStore store) =>
{
    take = Math.Clamp(take, 1, 200);

    string? normalizedLevel = null;
    if (!string.IsNullOrWhiteSpace(level))
    {
        if (!Enum.TryParse<LogLevel>(level, true, out var parsedLevel))
        {
            return Results.BadRequest(new
            {
                message = "Invalid level. Use Trace, Debug, Information, Warning, Error or Critical."
            });
        }

        normalizedLevel = parsedLevel.ToString();
    }

    var logs = store.ReadLatest(take, normalizedLevel);

    return Results.Ok(new
    {
        take,
        level = normalizedLevel,
        total = logs.Count,
        logs
    });
});

app.MapGet("/logs/providers", (IConfiguration configuration) =>
{
    var root = (IConfigurationRoot)configuration;
    var providers = root.Providers.Select(p => p.ToString()).ToArray();

    return Results.Ok(providers);
});

app.Run();

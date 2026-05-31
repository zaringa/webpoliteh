using Lab9.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConfigMeta:InMemorySource"] = "enabled"
});

builder.Services.Configure<AppSettingsOptions>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<GreetingOptions>(builder.Configuration.GetSection("Greeting"));
builder.Services.Configure<FeatureFlagsOptions>(builder.Configuration.GetSection("FeatureFlags"));

var app = builder.Build();

app.MapGet("/", (
    IWebHostEnvironment env,
    IOptionsSnapshot<GreetingOptions> greeting) =>
{
    return Results.Ok(new
    {
        env.EnvironmentName,
        greeting.Value.Message
    });
});

app.MapGet("/config", (
    IWebHostEnvironment env,
    IConfiguration configuration,
    IOptionsSnapshot<AppSettingsOptions> appSettings,
    IOptionsSnapshot<GreetingOptions> greeting,
    IOptionsSnapshot<FeatureFlagsOptions> flags) =>
{
    return Results.Ok(new
    {
        Environment = env.EnvironmentName,
        AppSettings = appSettings.Value,
        Greeting = greeting.Value,
        FeatureFlags = flags.Value,
        WebServer = new
        {
            UrlsFromConfig = configuration["urls"] ?? "(not set)",
            AspNetCoreUrls = configuration["ASPNETCORE_URLS"] ?? "(not set)"
        },
        ConnectionStrings = new
        {
            DefaultConnection = configuration.GetConnectionString("DefaultConnection")
        },
        Meta = new
        {
            InMemorySource = configuration["ConfigMeta:InMemorySource"]
        }
    });
});

app.MapGet("/config/providers", (IConfiguration configuration) =>
{
    var root = (IConfigurationRoot)configuration;

    var providers = root.Providers
        .Select(provider => provider.ToString())
        .ToArray();

    return Results.Ok(providers);
});

app.MapGet("/diagnostics", (
    IOptionsSnapshot<FeatureFlagsOptions> flags,
    IConfiguration configuration) =>
{
    if (!flags.Value.EnableDiagnostics)
    {
        return Results.StatusCode(StatusCodes.Status404NotFound);
    }

    return Results.Ok(new
    {
        Diagnostics = "enabled",
        ProcessId = Environment.ProcessId,
        DotNetEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "(not set)",
        Owner = configuration["AppSettings:Owner"]
    });
});

app.Run();

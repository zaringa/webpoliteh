using CachingLab.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMemoryCache();

builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024 * 1024;
    options.UseCaseSensitivePaths = false;
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
    options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "CachingLab:";
});

builder.Services.AddSingleton<ProductRepository>();

builder.Services.AddScoped<CacheInvalidationService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseResponseCaching();

app.MapControllers();

app.Run();
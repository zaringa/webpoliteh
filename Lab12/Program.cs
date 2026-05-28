using Lab12.Data;
using Lab12.Filters;
using Lab12.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<DatabaseExceptionFilter>();
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<DatabaseExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapGet("/", () => Results.Ok(new
{
    message = "Lab12 API is running",
    docs = "/swagger",
    tests = new[]
    {
        "ModelState business validation: POST /api/products with invalid discount",
        "Database filter: duplicate SKU on POST /api/products",
        "Global exception middleware: GET /api/errors/unhandled"
    }
}));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();

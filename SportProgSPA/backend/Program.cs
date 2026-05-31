using Microsoft.EntityFrameworkCore;
using SportProg.Api.Data;
using SportProg.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<SportProgDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=sportprog.db"));
builder.Services.AddScoped<PasswordService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Spa", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SportProgDbContext>();
    var passwords = scope.ServiceProvider.GetRequiredService<PasswordService>();
    await DatabaseSeeder.SeedAsync(db, passwords);
}

app.UseCors("Spa");
app.MapGet("/", () => Results.Ok(new { name = "SportProg API", status = "ready" }));
app.MapControllers();
app.Run();

using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.Name = ".StateLab.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapRazorPages();

app.MapPost("/api/state/name", (HttpContext context, UserDto dto) =>
{
    var name = NormalizeName(dto.Name);

    context.Response.Cookies.Append(
        "StateLab.UserName",
        WebUtility.UrlEncode(name),
        new CookieOptions
        {
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = context.Request.IsHttps,
            IsEssential = true
        });

    context.Session.SetString("StateLab.UserName", name);

    return Results.Ok(new
    {
        message = "Имя сохранено на сервере через API",
        savedName = name
    });
});

app.MapGet("/api/state/name", (HttpContext context) =>
{
    var cookieValue = context.Request.Cookies["StateLab.UserName"];
    var sessionValue = context.Session.GetString("StateLab.UserName");

    return Results.Ok(new
    {
        cookie = string.IsNullOrEmpty(cookieValue)
            ? null
            : WebUtility.UrlDecode(cookieValue),

        session = sessionValue
    });
});

app.Run();

static string NormalizeName(string? name)
{
    return string.IsNullOrWhiteSpace(name) ? "Гость" : name.Trim();
}

record UserDto(string? Name);

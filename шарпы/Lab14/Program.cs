var builder = WebApplication.CreateBuilder(args);

var corsEnabled = builder.Configuration.GetValue("Cors:Enabled", true);

var frontendReadOnlyOrigins = builder.Configuration
    .GetSection("Cors:FrontendReadOnlyOrigins")
    .Get<string[]>() ?? ["http://localhost:5500", "http://127.0.0.1:5500"];

var trustedSpaOrigins = builder.Configuration
    .GetSection("Cors:TrustedSpaOrigins")
    .Get<string[]>() ?? ["http://localhost:5173", "http://127.0.0.1:5173"];

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    if (corsEnabled)
    {
        options.AddPolicy("FrontendReadOnly", policy =>
        {
            policy.WithOrigins(frontendReadOnlyOrigins)
                .WithMethods("GET")
                .WithHeaders("Content-Type", "X-Client-Version");
        });

        options.AddPolicy("TrustedSpa", policy =>
        {
            policy.WithOrigins(trustedSpaOrigins)
                .WithMethods("GET", "POST", "PUT", "DELETE")
                .AllowAnyHeader()
                .AllowCredentials();
        });

        options.AddPolicy("AnyOriginGet", policy =>
        {
            policy.AllowAnyOrigin()
                .WithMethods("GET")
                .AllowAnyHeader();
        });

        return;
    }

    // "CORS disabled" mode: policies exist, but do not match real clients.
    var denyAllOrigins = new[] { "http://cors-disabled.local" };

    options.AddPolicy("FrontendReadOnly", policy =>
    {
        policy.WithOrigins(denyAllOrigins)
            .WithMethods("GET")
            .WithHeaders("Content-Type", "X-Client-Version");
    });

    options.AddPolicy("TrustedSpa", policy =>
    {
        policy.WithOrigins(denyAllOrigins)
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .AllowAnyHeader();
    });

    options.AddPolicy("AnyOriginGet", policy =>
    {
        policy.WithOrigins(denyAllOrigins)
            .WithMethods("GET")
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.MapGet("/", () => Results.Ok(new
{
    message = "Lab14 CORS demo is running",
    corsEnabled,
    policies = new
    {
        FrontendReadOnly = new
        {
            origins = frontendReadOnlyOrigins,
            methods = new[] { "GET" },
            headers = new[] { "Content-Type", "X-Client-Version" }
        },
        TrustedSpa = new
        {
            origins = trustedSpaOrigins,
            methods = new[] { "GET", "POST", "PUT", "DELETE" },
            headers = new[] { "*" },
            credentials = true
        },
        AnyOriginGet = new
        {
            origins = new[] { "*" },
            methods = new[] { "GET" },
            headers = new[] { "*" }
        }
    },
    endpoints = new[]
    {
        "/api/cors/no-policy",
        "/api/cors/frontend-read",
        "/api/cors/trusted",
        "/api/cors/open"
    }
}));

app.MapGet("/api/cors/no-policy", (HttpContext context) =>
{
    return Results.Ok(BuildPayload(context, "no-policy"));
});

app.MapGet("/api/cors/frontend-read", (HttpContext context) =>
{
    return Results.Ok(BuildPayload(context, "frontend-read/get"));
}).RequireCors("FrontendReadOnly");

app.MapPost("/api/cors/frontend-read", (HttpContext context) =>
{
    return Results.Ok(BuildPayload(context, "frontend-read/post"));
}).RequireCors("FrontendReadOnly");

app.MapGet("/api/cors/trusted", (HttpContext context) =>
{
    return Results.Ok(BuildPayload(context, "trusted/get"));
}).RequireCors("TrustedSpa");

app.MapGet("/api/cors/open", (HttpContext context) =>
{
    return Results.Ok(BuildPayload(context, "open/get"));
}).RequireCors("AnyOriginGet");

app.Run();

static object BuildPayload(HttpContext context, string endpoint)
{
    return new
    {
        endpoint,
        utc = DateTime.UtcNow,
        method = context.Request.Method,
        requestOrigin = context.Request.Headers.Origin.ToString(),
        note = "Use browser/external client to verify CORS behavior."
    };
}

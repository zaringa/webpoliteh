using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Text;
using System.Xml.Serialization;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDirectoryBrowser();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapGet("/html", () =>
{
    string html = "<h1>Привет, мир!</h1><p>Это HTML-страница, сгенерированная сервером.</p>";
    return Results.Content(html, "text/html; charset=utf-8");
})
.WithName("GetHtml")
.WithOpenApi();

app.MapGet("/text", () =>
{
    return Results.Text("Это простой текстовый ответ.", "text/plain; charset=utf-8");
})
.WithName("GetText")
.WithOpenApi();

app.MapGet("/json", () =>
{
    var data = new { Name = "ASP.NET Core", Version = "8.0", Description = "Пример JSON" };
    return Results.Json(data);
})
.WithName("GetJson")
.WithOpenApi();

app.MapGet("/xml", () =>
{
    var data = new Person { Name = "Иван", Age = 30 };
    var serializer = new XmlSerializer(typeof(Person));
    using var stringWriter = new StringWriter();
    serializer.Serialize(stringWriter, data);
    string xml = stringWriter.ToString();
    return Results.Content(xml, "application/xml; charset=utf-8");
})
.WithName("GetXml")
.WithOpenApi();

app.MapGet("/csv", () =>
{
    var csv = "Id,Name,Price\n1,Laptop,1200\n2,Mouse,25\n3,Keyboard,75";
    return Results.Content(csv, "text/csv; charset=utf-8");
})
.WithName("GetCsv")
.WithOpenApi();

app.MapGet("/binary", () =>
{
    byte[] binaryData = { 0x48, 0x65, 0x6C, 0x6C, 0x6F };
    return Results.Bytes(binaryData, "application/octet-stream", "hello.bin");
})
.WithName("GetBinary")
.WithOpenApi();

app.MapGet("/image", async () =>
{
    var filePath = Path.Combine(app.Environment.WebRootPath, "sample.png");
    if (!File.Exists(filePath))
    {
        return Results.NotFound("Файл изображения не найден");
    }
    var imageBytes = await File.ReadAllBytesAsync(filePath);
    return Results.File(imageBytes, "image/png");
})
.WithName("GetImage")
.WithOpenApi();

app.MapGet("/pdf", async () =>
{
    var filePath = Path.Combine(app.Environment.WebRootPath, "sample.pdf");
    if (!File.Exists(filePath))
    {
        return Results.NotFound("PDF-файл не найден");
    }
    var pdfBytes = await File.ReadAllBytesAsync(filePath);
    return Results.File(pdfBytes, "application/pdf", "document.pdf");
})
.WithName("GetPdf")
.WithOpenApi();

app.MapGet("/redirect", () =>
{
    return Results.Redirect("/html");
})
.WithName("GetRedirect")
.WithOpenApi();

app.MapGet("/redirect-permanent", () =>
{
    return Results.Redirect("/json", permanent: true);
})
.WithName("GetRedirectPermanent")
.WithOpenApi();

app.Run();

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () => "Hello World! Welcome to .NET 9 on Docker! develop by samphan.p");

app.MapGet("/ping", () => "pong");

app.MapGet("/api/health", () => new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    service = ".NET Docker App",
    version = "1.0.0"
});

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// เพิ่ม endpoint ใหม่สำหรับทดสอบ
app.MapGet("/api/products", () =>
{
    var products = new[]
    {
        new { Id = 1, Name = "Laptop", Price = 25000.00M, InStock = true },
        new { Id = 2, Name = "Mouse", Price = 500.00M, InStock = true },
        new { Id = 3, Name = "Keyboard", Price = 1200.00M, InStock = false },
        new { Id = 4, Name = "Monitor", Price = 8000.00M, InStock = true }
    };
    return Results.Ok(products);
})
.WithName("GetProducts");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
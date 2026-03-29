using NoviVovi.Infrastructure;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("NovelDatabase") ??
                 throw new ArgumentNullException("No such connection string");

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<NovelDatabaseService>(sp => 
    new NovelDatabaseService(connString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

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

// Запускаем тест асинхронно и ждем его выполнения
await RunDatabaseTest(connString);

// Запускаем приложение
await app.RunAsync();

async Task RunDatabaseTest(string connectionString)
{
    Console.WriteLine("=== НАЧАЛО ТЕСТИРОВАНИЯ БД ===\n");
    
    try
    {
        var test = new Test(connectionString);
        await test.TestDb();
        Console.WriteLine("\n=== ТЕСТИРОВАНИЕ ЗАВЕРШЕНО УСПЕШНО ===");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n=== ОШИБКА ТЕСТИРОВАНИЯ: {ex.Message} ===");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
    
    Console.WriteLine("\nНажмите Ctrl+C для остановки приложения...");
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Отдельный метод для тестирования БД
using NoviVovi.Infrastructure;
using Npgsql;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("NovelDatabase") ??
                 throw new ArgumentNullException("No such connection string");

builder.Services.AddApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddScoped<NovelDatabaseService>(sp => 
    new NovelDatabaseService(connString));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    // Генерирует файл спецификации (по умолчанию доступен по адресу /openapi/v1.json)
    app.MapOpenApi();
    
    // Красивый UI
    app.MapScalarApiReference(); 
}

app.UseHttpsRedirection();

app.MapControllers();

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
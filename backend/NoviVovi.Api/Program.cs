using NoviVovi.Api;
using NoviVovi.Application;
using NoviVovi.Infrastructure;
using NoviVovi.Infrastructure.DatabaseService;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

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

await RunDatabaseTest();

// Запускаем приложение
await app.RunAsync();

async Task RunDatabaseTest()
{
    Console.WriteLine("=== НАЧАЛО ТЕСТИРОВАНИЯ БД ===\n");
    
    try
    {
        using (var serviceScope = app.Services.CreateScope())
        {
            var test = new Test(serviceScope.ServiceProvider.GetService<NovelDatabaseService>());
            await test.TestDb();
            Console.WriteLine("\n=== ТЕСТИРОВАНИЕ ЗАВЕРШЕНО УСПЕШНО ===");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n=== ОШИБКА ТЕСТИРОВАНИЯ: {ex.Message} ===");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
    
    Console.WriteLine("\nНажмите Ctrl+C для остановки приложения...");
}
using System.Text.Json.Serialization;
using NoviVovi.Api;
using NoviVovi.Application;
using NoviVovi.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Это добавит кавычки и текст вместо цифр
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

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

// Запускаем приложение
await app.RunAsync();

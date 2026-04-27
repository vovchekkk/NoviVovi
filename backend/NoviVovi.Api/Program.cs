using System.Text.Json.Serialization;
using NoviVovi.Api;
using NoviVovi.Application;
using NoviVovi.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Укажите здесь URL вашего фронтенда
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
builder.Services.AddApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "NoviVovi API",
            Version = "v1",
            Description = "Visual Novel Engine API with polymorphic graph support"
        };
        return Task.CompletedTask;
    });
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");
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

// Make Program accessible for integration tests
public partial class Program { }

using System.Text.Json.Serialization;
using NoviVovi.Api;
using NoviVovi.Application;
using NoviVovi.Infrastructure;
using NoviVovi.Infrastructure.Mappers;
using NoviVovi.Infrastructure.Tests;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

// using (var scope = app.Services.CreateScope())
// {
//     var mapper = scope.ServiceProvider.GetRequiredService<LabelMapper>();
//     var test = new TestMapping(mapper);
//     test.Test1();
// }

// Запускаем приложение
await app.RunAsync();

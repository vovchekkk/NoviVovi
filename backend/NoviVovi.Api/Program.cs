using NoviVovi.Infrastructure;
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

app.Run();
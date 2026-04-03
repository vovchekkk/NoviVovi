using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Preview.Services;
using NoviVovi.Infrastructure.Labels;

namespace NoviVovi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("NovelDatabase") ??
                               throw new ArgumentNullException("No such connection string");
        
        services.AddScoped<NovelDatabaseService>(sp => 
            new NovelDatabaseService(connString));
        
        // services.AddScoped<INovelRepository, NovelRepository>();
        // services.AddScoped<ILabelRepository, LabelRepository>();
        
        //services.AddSingleton<Novels.Mappers.NovelDbMapper>();

        return services;
    }
}
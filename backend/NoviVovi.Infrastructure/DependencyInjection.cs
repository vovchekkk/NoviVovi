using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Infrastructure.DatabaseService;
using NoviVovi.Infrastructure.Mappers;

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

        services.AddSingleton<ImageMapper>();
        // services.AddSingleton<CharacterMapper>();
        // services.AddSingleton<CharacterStateMapper>();
        // services.AddScoped<INovelRepository, NovelRepository>();
        // services.AddScoped<ILabelRepository, LabelRepository>();
        
        //services.AddSingleton<Novels.Mappers.NovelDbMapper>();

        return services;
    }
}
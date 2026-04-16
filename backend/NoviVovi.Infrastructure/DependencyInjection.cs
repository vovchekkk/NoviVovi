using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Application.Common;
using NoviVovi.Application.Novels;
using NoviVovi.Infrastructure.Novels;

namespace NoviVovi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connString = configuration.GetConnectionString("NovelDatabase") ??
                               throw new ArgumentNullException("No such connection string");
        
        // services.AddScoped<NovelDatabaseService>(sp => 
        //     new NovelDatabaseService(connString));
        
        services.AddSingleton<INovelRepository, NovelRepository>();
        // services.AddSingleton<ILabelRepository, LabelRepository>();
        
        services.AddSingleton<IStorageService, S3StorageService>();
        services.AddSingleton<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
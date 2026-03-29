using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Preview.Services;
using NoviVovi.Infrastructure.Labels;
using NoviVovi.Infrastructure.Persistence;

namespace NoviVovi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        return services;
    }
    
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<PreviewSessionStore>();

        services.AddSingleton<ReplicaSnapshotMapper>();
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)); // Или UseSqlServer
        
        // services.AddScoped<INovelRepository, NovelRepository>();
        services.AddScoped<ILabelRepository, LabelRepository>();
        
        services.AddSingleton<Novels.Mappers.NovelDbMapper>();

        return services;
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Preview.Services;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Application.Transitions.Mappers;
using NoviVovi.Infrastructure.Novels;
using NoviVovi.Infrastructure.Labels;
using NoviVovi.Infrastructure.Persistence;

namespace NoviVovi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<PreviewSessionStore>();

        services.AddSingleton<ReplicaMapper>();
        services.AddSingleton<StepMapper>();
        services.AddSingleton<TransitionMapper>();
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)); // Или UseSqlServer
        
        services.AddScoped<INovelRepository, NovelRepository>();
        services.AddScoped<ILabelRepository, LabelRepository>();
        
        services.AddSingleton<Novels.Mappers.NovelDbMapper>();

        return services;
    }
}
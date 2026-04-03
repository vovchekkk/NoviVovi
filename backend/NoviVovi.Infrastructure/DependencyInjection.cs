using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Application.Labels;
using NoviVovi.Infrastructure.Labels;
using NoviVovi.Infrastructure.Persistence;

namespace NoviVovi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NovelDatabase")
                               ?? throw new ArgumentNullException("No such connection string");

        services.AddScoped<NovelDatabaseService>(sp =>
            new NovelDatabaseService(connectionString));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)); // Или UseSqlServer

        // services.AddScoped<INovelRepository, NovelRepository>();
        // services.AddScoped<ILabelRepository, LabelRepository>();

        services.AddSingleton<Novels.Mappers.NovelDbMapper>();

        return services;
    }
}
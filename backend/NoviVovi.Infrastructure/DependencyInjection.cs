using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Infrastructure.Mappers;
using NoviVovi.Infrastructure.Repositories;
using NoviVovi.Infrastructure.Repositories.DbO;
using NoviVovi.Infrastructure.Repositories.DbO.Interfaces;
using NoviVovi.Infrastructure.Repositories.New;

namespace NoviVovi.Infrastructure;

public class LazyResolver<T> : Lazy<T>
{
    public LazyResolver(IServiceProvider provider)
        : base(provider.GetRequiredService<T>)
    {
    }
}

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NovelDatabase") ??
                               throw new ArgumentNullException("No such connection string");
        
        services.AddScoped(typeof(Lazy<>), typeof(LazyResolver<>));

        services.AddSingleton(new DatabaseOptions(connectionString));
        
        services.AddScoped<CharacterMapper>();
        services.AddScoped<ImageMapper>();
        services.AddScoped<LabelMapper>();
        services.AddScoped<MenuMapper>();
        services.AddScoped<NovelMapper>();
        services.AddScoped<ReplicaMapper>();
        services.AddScoped<StepMapper>();
        services.AddScoped<TransformMapper>();

        services.AddScoped<ICharacterDbORepository, CharacterDbORepository>();
        services.AddScoped<IImageDbORepository, ImageDbORepository>();
        services.AddScoped<ILabelDbORepository, LabelDbORepository>();
        services.AddScoped<IMenuDbORepository, MenuDbORepository>();
        services.AddScoped<INovelDbORepository, NovelDbORepository>();
        services.AddScoped<IStepDbORepository, StepDbORepository>();

        services.AddScoped<INovelRepository, NovelRepository>();
        services.AddScoped<ILabelRepository, LabelRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();

        services.AddSingleton<IStorageService, S3StorageService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
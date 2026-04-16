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

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NovelDatabase") ??
                               throw new ArgumentNullException("No such connection string");

        services.AddSingleton(new DatabaseOptions(connectionString));

        services.AddSingleton<CharacterMapper>();
        services.AddSingleton<ImageMapper>();
        services.AddSingleton<LabelMapper>();
        services.AddSingleton<MenuMapper>();
        services.AddSingleton<NovelMapper>();
        services.AddSingleton<ReplicaMapper>();
        services.AddSingleton<StepMapper>();
        services.AddSingleton<TransformMapper>();

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
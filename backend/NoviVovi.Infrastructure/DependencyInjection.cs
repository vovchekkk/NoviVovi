using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Export.Abstractions;
using NoviVovi.Application.Images.Abstractions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Characters.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Labels.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Novels.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Services;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Archive;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Images;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Resources;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Script;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;
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

        // RenPy Exporter - SOLID compliant decomposition
        services.AddScoped<RenPyIdentifierGenerator>();
        services.AddScoped<CharacterToRenPyMapper>();
        services.AddScoped<LabelToRenPyMapper>();
        services.AddScoped<NovelToRenPyMapper>();
        services.AddScoped<StepToRenPyMapper>();
        services.AddScoped<TransformToRenPyMapper>();
        
        // RenPy Services - each with single responsibility
        services.AddSingleton<IEmbeddedResourceLoader, EmbeddedResourceLoader>();
        services.AddScoped<IRenPyStatementRenderer, RenPyStatementRenderer>();
        services.AddScoped<IRenPyScriptGenerator, RenPyScriptGenerator>();
        services.AddScoped<INovelImageCollector, NovelImageCollector>();
        services.AddScoped<IImageExporter, RenPyImageExporter>();
        services.AddScoped<IRenPyArchiveBuilder, RenPyArchiveBuilder>();
        services.AddScoped<IExporter, RenPyExporter>();

        return services;
    }
}
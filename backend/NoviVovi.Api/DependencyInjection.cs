using MediatR;
using NoviVovi.Api.Characters.CommandMappers;
using NoviVovi.Api.Characters.Mappers;
using NoviVovi.Api.Dialogue.Mappers;
using NoviVovi.Api.Images.CommandMappers;
using NoviVovi.Api.Images.Mappers;
using NoviVovi.Api.Labels.CommandMappers;
using NoviVovi.Api.Labels.Mappers;
using NoviVovi.Api.Menu.Mappers;
using NoviVovi.Api.Novels.CommandMappers;
using NoviVovi.Api.Novels.Mappers;
using NoviVovi.Api.Preview.Mappers;
using NoviVovi.Api.Scene.Mappers;
using NoviVovi.Api.Steps.CommandMappers;
using NoviVovi.Api.Steps.Mappers;
using NoviVovi.Api.Transitions.Mappers;
using NoviVovi.Application.Novels.Models;

namespace NoviVovi.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddMediatR(cfg => {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName?.StartsWith("NoviVovi") ?? false)
                .ToArray();
            cfg.RegisterServicesFromAssemblies(assemblies);
        });
        
        services.AddSingleton<CharacterCommandMapper>();
        services.AddSingleton<CharacterStateCommandMapper>();
        
        services.AddSingleton<CharacterResponseMapper>();
        services.AddSingleton<CharacterStateResponseMapper>();
        
        services.AddSingleton<ReplicaResponseMapper>();
        
        services.AddSingleton<ImageCommandMapper>();
        
        services.AddSingleton<ImageResponseMapper>();
        services.AddSingleton<UploadInfoImageResponseMapper>();
        
        services.AddSingleton<LabelCommandMapper>();
        
        services.AddSingleton<LabelResponseMapper>();
        
        services.AddSingleton<ChoiceResponseMapper>();
        services.AddSingleton<MenuResponseMapper>();
        
        services.AddSingleton<NovelCommandMapper>();
        
        services.AddSingleton<EdgeResponseMapper>();
        services.AddSingleton<NodeResponseMapper>();
        services.AddSingleton<NovelGraphResponseMapper>();
        services.AddSingleton<NovelResponseMapper>();
        
        services.AddSingleton<SceneStateResponseMapper>();
        
        services.AddSingleton<BackgroundObjectResponseMapper>();
        services.AddSingleton<CharacterObjectResponseMapper>();
        services.AddSingleton<PositionResponseMapper>();
        services.AddSingleton<SizeResponseMapper>();
        services.AddSingleton<TransformRequestMapper>();
        services.AddSingleton<TransformResponseMapper>();
        
        services.AddSingleton<StepCommandMapper>();
        
        services.AddSingleton<StepResponseMapper>();
        services.AddSingleton<NovelGraphBuilder>();
        services.AddSingleton<TransitionResponseMapper>();
        
        return services;
    }
}
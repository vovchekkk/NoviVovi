using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Menu.Mappers;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Mappers;
using NoviVovi.Application.Transitions.Mappers;

namespace NoviVovi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<CharacterDtoMapper>();
        services.AddSingleton<CharacterStateDtoMapper>();
        
        services.AddSingleton<ReplicaDtoMapper>();
        
        services.AddSingleton<ImageDtoMapper>();
        services.AddSingleton<UploadInfoImageDtoMapper>();
        
        services.AddSingleton<LabelDtoMapper>();
        
        services.AddSingleton<ChoiceDtoMapper>();
        services.AddSingleton<MenuDtoMapper>();
        
        services.AddSingleton<ChoiceEdgeDtoMapper>();
        services.AddSingleton<EdgeDtoMapper>();
        services.AddSingleton<JumpEdgeDtoMapper>();
        services.AddSingleton<NodeDtoMapper>();
        services.AddSingleton<NovelDtoMapper>();
        services.AddSingleton<NovelGraphDtoMapper>();
        
        services.AddSingleton<SceneStateDtoMapper>();
        
        services.AddSingleton<BackgroundObjectDtoMapper>();
        services.AddSingleton<CharacterObjectDtoMapper>();
        services.AddSingleton<PositionDtoMapper>();
        services.AddSingleton<SizeDtoMapper>();
        services.AddSingleton<TransformDtoMapper>();
        
        services.AddSingleton<StepDtoMapper>();
        
        services.AddSingleton<ChoiceTransitionDtoMapper>();
        services.AddSingleton<JumpTransitionDtoMapper>();
        services.AddSingleton<NextStepTransitionDtoMapper>();
        
        return services;
    }
}
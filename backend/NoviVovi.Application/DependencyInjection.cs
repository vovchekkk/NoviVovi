using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Menu.Mappers;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Preview.Services;
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
        
        services.AddSingleton<LabelDtoMapper>();
        
        services.AddSingleton<ChoiceDtoMapper>();
        services.AddSingleton<MenuDtoMapper>();
        
        services.AddSingleton<NovelDtoMapper>();
        
        services.AddSingleton<SceneStateDtoMapper>();
        
        services.AddSingleton<PreviewSessionStore>();
        
        services.AddSingleton<BackgroundObjectDtoMapper>();
        services.AddSingleton<CharacterObjectDtoMapper>();
        services.AddSingleton<PositionDtoMapper>();
        services.AddSingleton<SizeDtoMapper>();
        services.AddSingleton<TransformDtoMapper>();
        
        services.AddSingleton<HideCharacterStepDtoMapper>();
        services.AddSingleton<JumpStepDtoMapper>();
        services.AddSingleton<ShowBackgroundStepDtoMapper>();
        services.AddSingleton<ShowCharacterStepDtoMapper>();
        services.AddSingleton<ShowMenuStepDtoMapper>();
        services.AddSingleton<ShowReplicaStepDtoMapper>();
        
        services.AddSingleton<ChoiceTransitionDtoMapper>();
        services.AddSingleton<JumpTransitionDtoMapper>();
        services.AddSingleton<NextStepTransitionDtoMapper>();
        
        return services;
    }
}
using NoviVovi.Api.Characters.CommandMappers;
using NoviVovi.Api.Characters.Mappers;
using NoviVovi.Api.Dialogue.Mappers;
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

namespace NoviVovi.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddSingleton<AddCharacterCommandMapper>();
        services.AddSingleton<AddCharacterStateCommandMapper>();
        services.AddSingleton<PatchCharacterCommandMapper>();
        services.AddSingleton<PatchCharacterStateCommandMapper>();
        
        services.AddSingleton<CharacterResponseMapper>();
        services.AddSingleton<CharacterStateResponseMapper>();
        
        services.AddSingleton<ReplicaResponseMapper>();
        
        services.AddSingleton<ImageResponseMapper>();
        
        services.AddSingleton<AddLabelCommandMapper>();
        services.AddSingleton<PatchLabelCommandMapper>();
        
        services.AddSingleton<LabelResponseMapper>();
        
        services.AddSingleton<ChoiceResponseMapper>();
        services.AddSingleton<MenuResponseMapper>();
        
        services.AddSingleton<CreateNovelCommandMapper>();
        services.AddSingleton<PatchNovelCommandMapper>();
        
        services.AddSingleton<NovelResponseMapper>();
        
        services.AddSingleton<SceneStateResponseMapper>();
        
        services.AddSingleton<BackgroundObjectResponseMapper>();
        services.AddSingleton<CharacterObjectResponseMapper>();
        services.AddSingleton<PositionResponseMapper>();
        services.AddSingleton<SizeResponseMapper>();
        services.AddSingleton<TransformRequestMapper>();
        services.AddSingleton<TransformResponseMapper>();
        
        services.AddSingleton<AddStepCommandMapper>();
        services.AddSingleton<PatchStepCommandMapper>();
        
        services.AddSingleton<StepResponseMapper>();
        
        services.AddSingleton<ChoiceTransitionResponseMapper>();
        services.AddSingleton<JumpTransitionResponseMapper>();
        services.AddSingleton<NextStepTransitionResponseMapper>();
        
        return services;
    }
}
using NoviVovi.Api.Characters.Mappers;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Dialogue.Mappers;
using NoviVovi.Api.Images.Mappers;
using NoviVovi.Api.Menu.Mappers;
using NoviVovi.Api.Scene.Mappers;
using NoviVovi.Api.Scene.Responses;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class StepResponseMapper(
    CharacterResponseMapper characterMapper,
    CharacterStateResponseMapper characterStateMapper,
    TransformResponseMapper transformMapper,
    ReplicaResponseMapper replicaMapper,
    ChoiceResponseMapper choiceMapper,
    ImageResponseMapper imageMapper
)
{
    [MapDerivedType(typeof(HideCharacterStepDto), typeof(HideCharacterStepResponse))]
    [MapDerivedType(typeof(JumpStepDto), typeof(JumpStepResponse))]
    [MapDerivedType(typeof(ShowBackgroundStepDto), typeof(ShowBackgroundStepResponse))]
    [MapDerivedType(typeof(ShowCharacterStepDto), typeof(ShowCharacterStepResponse))]
    [MapDerivedType(typeof(ShowMenuStepDto), typeof(ShowMenuStepResponse))]
    [MapDerivedType(typeof(ShowReplicaStepDto), typeof(ShowReplicaStepResponse))]
    public partial StepResponse ToResponse(StepDto source);
    
    public partial IEnumerable<StepResponse> ToResponses(IEnumerable<StepDto> source);
    
    // Explicit implementations for each step type
    public JumpStepResponse ToResponse(JumpStepDto source) => new()
    {
        Id = source.Id,
        Transition = new JumpTransitionResponse 
        { 
            TargetLabelId = source.Transition.TargetLabelId
        }
    };
    
    public ShowBackgroundStepResponse ToResponse(ShowBackgroundStepDto source) => new()
    {
        Id = source.Id,
        BackgroundObject = new BackgroundObjectResponse(
            Id: source.BackgroundObject.Id,
            Image: imageMapper.ToResponse(source.BackgroundObject.Image),
            Transform: transformMapper.ToResponse(source.BackgroundObject.Transform)
        ),
        Transition = new NextStepTransitionResponse()
    };
    
    public ShowCharacterStepResponse ToResponse(ShowCharacterStepDto source) => new()
    {
        Id = source.Id,
        CharacterObject = new CharacterObjectResponse(
            Id: source.CharacterObject.Id,
            Character: characterMapper.ToResponse(source.CharacterObject.Character),
            State: characterStateMapper.ToResponse(source.CharacterObject.State),
            Transform: transformMapper.ToResponse(source.CharacterObject.Transform)
        ),
        Transition = new NextStepTransitionResponse()
    };
    
    public ShowMenuStepResponse ToResponse(ShowMenuStepDto source) => new()
    {
        Id = source.Id,
        Menu = new Menu.Responses.MenuResponse(
            Choices: choiceMapper.ToResponses(source.Menu.Choices).ToList()
        ),
        Transition = new NextStepTransitionResponse()
    };

    public ShowReplicaStepResponse ToResponse(ShowReplicaStepDto source) => new()
    {
        Id = source.Id,
        Replica = replicaMapper.ToResponse(source.Replica),
        Transition = new NextStepTransitionResponse()
    };
    
    public HideCharacterStepResponse ToResponse(HideCharacterStepDto source) => new()
    {
        Id = source.Id,
        Character = characterMapper.ToResponse(source.Character),
        Transition = new NextStepTransitionResponse()
    };
}

using NoviVovi.Api.Characters.Mappers;
using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Scene.Mappers;
using NoviVovi.Api.Scene.Responses;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class StepResponseMapper(
    CharacterResponseMapper characterMapper,
    TransformResponseMapper transformMapper
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
    
    public partial HideCharacterStepResponse ToResponse(HideCharacterStepDto source);
    
    public JumpStepResponse ToResponse(JumpStepDto source) => new()
    {
        Id = source.Id,
        Transition = new JumpTransitionResponse 
        { 
            Id = source.Transition.Id, 
            TargetLabelId = source.Transition.TargetLabelId
        }
    };
    
    public partial ShowBackgroundStepResponse ToResponse(ShowBackgroundStepDto source);
    
    public partial ShowCharacterStepResponse ToResponse(ShowCharacterStepDto source);
    
    public partial ShowMenuStepResponse ToResponse(ShowMenuStepDto source);

    public partial ShowReplicaStepResponse ToResponse(ShowReplicaStepDto source);
    
    private TransformResponse MapTransform(TransformDto source) => transformMapper.ToResponse(source);
    
    private CharacterResponse MapCharacter(CharacterDto source) => characterMapper.ToResponse(source);
}
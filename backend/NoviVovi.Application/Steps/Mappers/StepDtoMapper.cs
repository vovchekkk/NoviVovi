using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Transitions.Dtos;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class StepDtoMapper(
    CharacterDtoMapper characterMapper,
    TransformDtoMapper transformMapper
)
{
    [MapDerivedType(typeof(HideCharacterStep), typeof(HideCharacterStepDto))]
    [MapDerivedType(typeof(JumpStep), typeof(JumpStepDto))]
    [MapDerivedType(typeof(ShowBackgroundStep), typeof(ShowBackgroundStepDto))]
    [MapDerivedType(typeof(ShowCharacterStep), typeof(ShowCharacterStepDto))]
    [MapDerivedType(typeof(ShowMenuStep), typeof(ShowMenuStepDto))]
    [MapDerivedType(typeof(ShowReplicaStep), typeof(ShowReplicaStepDto))]
    public partial StepDto ToDto(Step source);
    
    public partial IEnumerable<StepDto> ToDtos(IEnumerable<Step> source);
    
    public JumpStepDto ToDto(JumpStep source) => new()
    {
        Id = source.Id,
        Transition = new JumpTransitionDto 
        { 
            TargetLabelId = source.Transition.TargetLabel.Id
        }
    };
    
    public partial ShowBackgroundStepDto ToDto(ShowBackgroundStep source);
    
    public partial ShowCharacterStepDto ToDto(ShowCharacterStep source);
    
    public partial ShowMenuStepDto ToDto(ShowMenuStep source);

    public partial ShowReplicaStepDto ToDto(ShowReplicaStep source);
    
    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);
    
    private CharacterDto MapCharacter(Character source) => characterMapper.ToDto(source);
}
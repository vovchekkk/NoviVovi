using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class StepDtoMapper(
    ReplicaDtoMapper replicaMapper,
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
    public partial StepDto ToDto(Step subject);

    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);
    private ReplicaDto MapReplica(Replica source) => replicaMapper.ToDto(source);
    private CharacterDto MapCharacter(Character source) => characterMapper.ToDto(source);
}
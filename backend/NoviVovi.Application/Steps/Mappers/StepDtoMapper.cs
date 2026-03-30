using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Steps;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Steps.Mappers;

[Mapper]
public partial class StepDtoMapper
{
    [MapDerivedType(typeof(HideCharacterStep), typeof(HideCharacterStepDto))]
    [MapDerivedType(typeof(JumpStep), typeof(JumpStepDto))]
    [MapDerivedType(typeof(ShowBackgroundStep), typeof(ShowBackgroundStepDto))]
    [MapDerivedType(typeof(ShowCharacterStep), typeof(ShowCharacterStepDto))]
    [MapDerivedType(typeof(ShowMenuStep), typeof(ShowMenuStepDto))]
    [MapDerivedType(typeof(ShowReplicaStep), typeof(ShowReplicaStepDto))]
    public partial StepDto ToDto(Step subject);
}
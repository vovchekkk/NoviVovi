using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class StepResponseMapper
{
    [MapDerivedType(typeof(HideCharacterStepDto), typeof(HideCharacterStepResponse))]
    [MapDerivedType(typeof(JumpStepDto), typeof(JumpStepResponse))]
    [MapDerivedType(typeof(ShowBackgroundStepDto), typeof(ShowBackgroundStepResponse))]
    [MapDerivedType(typeof(ShowCharacterStepDto), typeof(ShowCharacterStepResponse))]
    [MapDerivedType(typeof(ShowMenuStepDto), typeof(ShowMenuStepResponse))]
    [MapDerivedType(typeof(ShowReplicaStepDto), typeof(ShowReplicaStepResponse))]
    public partial StepResponse ToResponse(StepDto subject);
    
    public partial IEnumerable<StepResponse> ToResponse(IEnumerable<StepDto> subjects);
}
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class HideCharacterStepMapper
{
    public partial HideCharacterStepResponse ToResponse(HideCharacterStepDto subject);
    
    public partial IEnumerable<HideCharacterStepResponse> ToResponses(IEnumerable<HideCharacterStepDto> subjects);
}
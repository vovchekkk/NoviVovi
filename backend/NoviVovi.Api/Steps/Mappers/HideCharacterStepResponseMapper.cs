using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class HideCharacterStepMapper
{
    public partial HideCharacterStepResponse ToResponse(HideCharacterStepSnapshot subject);
    
    public partial IEnumerable<HideCharacterStepResponse> ToResponses(IEnumerable<HideCharacterStepSnapshot> subjects);
}
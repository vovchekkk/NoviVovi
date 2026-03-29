using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class ShowCharacterStepMapper
{
    public partial ShowCharacterStepResponse ToResponse(ShowCharacterStepDto subject);
    
    public partial IEnumerable<ShowCharacterStepResponse> ToResponses(IEnumerable<ShowCharacterStepDto> subjects);
}
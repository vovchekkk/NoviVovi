using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class ShowCharacterStepMapper
{
    public partial ShowCharacterStepResponse ToResponse(ShowCharacterStepSnapshot subject);
    
    public partial IEnumerable<ShowCharacterStepResponse> ToResponses(IEnumerable<ShowCharacterStepSnapshot> subjects);
}
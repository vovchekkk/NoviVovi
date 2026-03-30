using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class ShowMenuStepResponseMapper
{
    public partial ShowMenuStepResponse ToResponse(ShowMenuStepDto subject);
    
    public partial IEnumerable<ShowMenuStepResponse> ToResponses(IEnumerable<ShowMenuStepDto> subjects);
}
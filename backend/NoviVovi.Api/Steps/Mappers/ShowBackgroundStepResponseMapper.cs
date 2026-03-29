using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class ShowBackgroundStepMapper
{
    public partial ShowBackgroundStepResponse ToResponse(ShowBackgroundStepDto subject);
    
    public partial IEnumerable<ShowBackgroundStepResponse> ToResponses(IEnumerable<ShowBackgroundStepDto> subjects);
}
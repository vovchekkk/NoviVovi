using NoviVovi.Api.Steps.Responses;
using NoviVovi.Application.Steps.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Steps.Mappers;

[Mapper]
public partial class ShowBackgroundStepMapper
{
    public partial ShowBackgroundStepResponse ToResponse(ShowBackgroundStepSnapshot subject);
    
    public partial IEnumerable<ShowBackgroundStepResponse> ToResponses(IEnumerable<ShowBackgroundStepSnapshot> subjects);
}
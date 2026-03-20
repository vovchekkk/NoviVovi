using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Transitions.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Transitions.Mappers;

[Mapper]
public partial class NextStepTransitionResponseMapper
{
    public partial NextStepTransitionResponse ToResponse(NextStepTransitionSnapshot subject);
    
    public partial IEnumerable<NextStepTransitionResponse> ToResponses(IEnumerable<NextStepTransitionSnapshot> subjects);
}
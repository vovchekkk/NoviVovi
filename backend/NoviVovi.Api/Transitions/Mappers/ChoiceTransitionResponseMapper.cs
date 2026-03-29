using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Transitions.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Transitions.Mappers;

[Mapper]
public partial class ChoiceTransitionResponseMapper
{
    public partial ChoiceTransitionResponse ToResponse(ChoiceTransitionDto subject);
    
    public partial IEnumerable<ChoiceTransitionResponse> ToResponses(IEnumerable<ChoiceTransitionDto> subjects);
}
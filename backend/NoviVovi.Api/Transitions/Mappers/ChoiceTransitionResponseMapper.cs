using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Transitions.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Transitions.Mappers;

[Mapper]
public partial class ChoiceTransitionResponseMapper
{
    public partial ChoiceTransitionResponse ToResponse(ChoiceTransitionSnapshot novel);
    
    public partial IEnumerable<ChoiceTransitionResponse> ToResponses(IEnumerable<ChoiceTransitionSnapshot> novels);
}
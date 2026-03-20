using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Transitions.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Transitions.Mappers;

[Mapper]
public partial class JumpTransitionResponseMapper
{
    public partial JumpTransitionResponse ToResponse(JumpTransitionSnapshot subject);
    
    public partial IEnumerable<JumpTransitionResponse> ToResponses(IEnumerable<JumpTransitionSnapshot> subjects);
}
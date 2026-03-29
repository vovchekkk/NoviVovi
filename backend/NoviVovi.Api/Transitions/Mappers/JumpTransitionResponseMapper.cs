using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Transitions.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Transitions.Mappers;

[Mapper]
public partial class JumpTransitionResponseMapper
{
    public partial JumpTransitionResponse ToResponse(JumpTransitionDto subject);
    
    public partial IEnumerable<JumpTransitionResponse> ToResponses(IEnumerable<JumpTransitionDto> subjects);
}
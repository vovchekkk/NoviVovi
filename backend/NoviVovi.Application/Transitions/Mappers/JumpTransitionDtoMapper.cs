using NoviVovi.Application.Transitions.Dtos;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class JumpTransitionDtoMapper
{
    public partial JumpTransitionDto ToDto(JumpTransition? subject);
    
    public partial IEnumerable<JumpTransitionDto> ToDtos(IEnumerable<JumpTransition> subjects);
}
using NoviVovi.Application.Transitions.Dtos;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class NextStepTransitionDtoMapper
{
    public partial NextStepTransitionDto ToDto(NextStepTransition subject);
    
    public partial IEnumerable<NextStepTransitionDto> ToDtos(IEnumerable<NextStepTransition> subjects);
}
using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Application.Transitions.Dtos;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class ChoiceTransitionDtoMapper
{
    public partial ChoiceTransitionDto ToDto(ChoiceTransition subject);
    
    public partial IEnumerable<ChoiceTransitionDto> ToDtos(IEnumerable<ChoiceTransition> subjects);
}
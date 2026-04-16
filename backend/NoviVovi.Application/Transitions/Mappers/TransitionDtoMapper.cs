using NoviVovi.Application.Transitions.Dtos;
using NoviVovi.Domain.Transitions;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Transitions.Mappers;

[Mapper]
public partial class TransitionDtoMapper
{
    [MapDerivedType(typeof(ChoiceTransition), typeof(ChoiceTransitionDto))]
    [MapDerivedType(typeof(JumpTransition), typeof(JumpTransitionDto))]
    [MapDerivedType(typeof(NextStepTransition), typeof(NextStepTransitionDto))]
    public partial TransitionDto ToDto(Transition source);
}
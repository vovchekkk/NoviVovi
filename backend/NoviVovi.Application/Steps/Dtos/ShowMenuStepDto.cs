using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record ShowMenuStepDto(
    Guid Id,
    MenuDto Menu,
    TransitionDto Transition
) : StepDto(Id, Transition);
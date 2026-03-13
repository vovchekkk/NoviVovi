using NoviVovi.Application.Menu.Contracts;
using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record ShowMenuStepSnapshot(
    Guid Id,
    MenuSnapshot Menu,
    TransitionSnapshot Transition
) : StepSnapshot(Id, Transition);
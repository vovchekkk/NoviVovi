using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record StepSnapshot(
    Guid Id,
    TransitionSnapshot Transition
);
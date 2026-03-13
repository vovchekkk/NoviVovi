using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public abstract record StepSnapshot(
    Guid Id,
    TransitionSnapshot Transition
);
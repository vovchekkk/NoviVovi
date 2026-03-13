using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public abstract record StepSnapshot<TTransition>(
    Guid Id,
    TTransition Transition
) where TTransition : TransitionSnapshot;
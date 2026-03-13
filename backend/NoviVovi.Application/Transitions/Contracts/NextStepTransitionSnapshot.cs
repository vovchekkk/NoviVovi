namespace NoviVovi.Application.Transitions.Contracts;

public record NextStepTransitionSnapshot(
    Guid Id
) : TransitionSnapshot(Id);
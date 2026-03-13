namespace NoviVovi.Application.Transitions.Contracts;

public record JumpTransitionSnapshot(
    Guid Id,
    Guid TargetLabelId
) : TransitionSnapshot(Id);
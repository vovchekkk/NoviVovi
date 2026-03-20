using NoviVovi.Application.Labels.Contracts;

namespace NoviVovi.Application.Transitions.Contracts;

public record JumpTransitionSnapshot(
    Guid Id,
    LabelSnapshot TargetLabel
) : TransitionSnapshot(Id);
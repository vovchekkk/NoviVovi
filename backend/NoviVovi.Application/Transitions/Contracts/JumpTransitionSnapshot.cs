using NoviVovi.Application.Labels.Contracts;

namespace NoviVovi.Application.Transitions.Contracts;

public record JumpTransitionSnapshot(
    Guid TargetLabelId
);
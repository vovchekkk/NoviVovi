using NoviVovi.Application.Labels.Contracts;

namespace NoviVovi.Application.Transitions.Contracts;

public record ChoiceTransitionSnapshot(
    Guid Id,
    LabelSnapshot TargetLabel
) : TransitionSnapshot(Id);
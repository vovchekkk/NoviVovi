namespace NoviVovi.Application.Transitions.Contracts;

public record ChoiceTransitionSnapshot(
    Guid Id,
    Guid TargetLabelId
) : TransitionSnapshot(Id);
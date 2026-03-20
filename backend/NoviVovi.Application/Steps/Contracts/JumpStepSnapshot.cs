using NoviVovi.Application.Labels.Contracts;
using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record JumpStepSnapshot(
    Guid Id,
    LabelSnapshot TargetLabel,
    TransitionSnapshot Transition
) : StepSnapshot(Id, Transition);
using NoviVovi.Application.Images.Contracts;
using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record ShowBackgroundStepSnapshot(
    Guid Id,
    BackgroundObjectSnapshot BackgroundObject,
    TransitionSnapshot Transition
) : StepSnapshot(Id, Transition);
using NoviVovi.Application.Images.Contracts;
using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record ShowBackgroundStepSnapshot(
    Guid Id,
    ImageSnapshot Background,
    TransformSnapshot Transform,
    TransitionSnapshot Transition
) : StepSnapshot(Id, Transition);
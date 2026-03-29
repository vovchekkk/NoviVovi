using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record ShowBackgroundStepDto(
    Guid Id,
    BackgroundObjectDto BackgroundObject,
    TransitionDto Transition
) : StepDto(Id, Transition);
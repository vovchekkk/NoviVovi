using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record ShowCharacterStepDto(
    Guid Id,
    CharacterObjectDto CharacterObject,
    TransitionDto Transition
) : StepDto(Id, Transition);
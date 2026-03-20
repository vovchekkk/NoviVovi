using NoviVovi.Application.Characters.Contracts;
using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record ShowCharacterStepSnapshot(
    Guid Id,
    CharacterObjectSnapshot CharacterObject,
    TransitionSnapshot Transition
) : StepSnapshot(Id, Transition);
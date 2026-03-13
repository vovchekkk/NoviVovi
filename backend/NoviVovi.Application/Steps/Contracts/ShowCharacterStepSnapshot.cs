using NoviVovi.Application.Characters.Contracts;
using NoviVovi.Application.Scene.Contracts;
using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record ShowCharacterStepSnapshot(
    Guid Id,
    CharacterSnapshot Character,
    CharacterStateSnapshot State,
    TransformSnapshot Transform,
    TransitionSnapshot Transition
) : StepSnapshot (Id, Transition);
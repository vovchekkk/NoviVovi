using NoviVovi.Application.Characters.Contracts;
using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record HideCharacterStepSnapshot(
    Guid Id,
    CharacterSnapshot Character,
    NextStepTransitionSnapshot Transition
) : StepSnapshot<NextStepTransitionSnapshot>(Id, Transition);
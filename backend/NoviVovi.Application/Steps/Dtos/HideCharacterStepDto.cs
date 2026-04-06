using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record HideCharacterStepDto(
    Guid Id,
    Guid CharacterId,
    TransitionDto Transition
) : StepDto(Id, Transition);
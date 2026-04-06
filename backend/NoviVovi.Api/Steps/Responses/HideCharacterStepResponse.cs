using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record HideCharacterStepResponse(
    Guid Id,
    Guid CharacterId,
    TransitionResponse Transition
) : StepResponse(Id, Transition);
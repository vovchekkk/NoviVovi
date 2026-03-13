using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Scene.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowCharacterStepResponse(
    Guid Id,
    CharacterResponse Character,
    CharacterStateResponse State,
    TransformResponse Transform,
    TransitionResponse Transition
) : StepResponse (Id, Transition);
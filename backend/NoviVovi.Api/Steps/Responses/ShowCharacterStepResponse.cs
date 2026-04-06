using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Scene.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowCharacterStepResponse : StepResponse<NextStepTransitionResponse>
{
    public required CharacterObjectResponse CharacterObject { get; init; }
}
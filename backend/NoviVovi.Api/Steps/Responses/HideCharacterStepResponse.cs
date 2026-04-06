using NoviVovi.Api.Characters.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record HideCharacterStepResponse : StepResponse<NextStepTransitionResponse>
{
    public required Guid CharacterId { get; init; }
}
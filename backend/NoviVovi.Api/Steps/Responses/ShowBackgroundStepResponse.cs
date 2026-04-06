using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Scene.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowBackgroundStepResponse : StepResponse<NextStepTransitionResponse>
{
    public required BackgroundObjectResponse BackgroundObject { get; init; }
}
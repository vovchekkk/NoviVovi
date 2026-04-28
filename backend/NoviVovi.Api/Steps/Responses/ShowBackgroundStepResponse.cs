using System.Text.Json.Serialization;
using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Infrastructure;
using NoviVovi.Api.Scene.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowBackgroundStepResponse : StepResponse
{
    public required BackgroundObjectResponse BackgroundObject { get; init; }
    
    [JsonConverter(typeof(TransitionResponseConverter))]
    public required TransitionResponse Transition { get; init; }
}

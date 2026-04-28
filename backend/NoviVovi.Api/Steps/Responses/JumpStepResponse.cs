using System.Text.Json.Serialization;
using NoviVovi.Api.Infrastructure;
using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record JumpStepResponse : StepResponse
{
    [JsonConverter(typeof(TransitionResponseConverter))]
    public required TransitionResponse Transition { get; init; }
}

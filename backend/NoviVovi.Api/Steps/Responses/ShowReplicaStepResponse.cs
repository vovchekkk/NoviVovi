using System.Text.Json.Serialization;
using NoviVovi.Api.Dialogue.Responses;
using NoviVovi.Api.Infrastructure;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowReplicaStepResponse : StepResponse
{
    public required ReplicaResponse Replica { get; init; }
    
    [JsonConverter(typeof(TransitionResponseConverter))]
    public required TransitionResponse Transition { get; init; }
}

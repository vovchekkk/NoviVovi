using NoviVovi.Api.Dialogue.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowReplicaStepResponse : StepResponse<NextStepTransitionResponse>
{
    public required ReplicaResponse Replica { get; init; }
}
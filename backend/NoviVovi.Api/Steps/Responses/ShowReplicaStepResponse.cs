using NoviVovi.Api.Dialogue.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record ShowReplicaStepResponse(
    Guid Id,
    ReplicaResponse Replica,
    TransitionResponse Transition
) : StepResponse(Id, Transition);
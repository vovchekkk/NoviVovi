using NoviVovi.Application.Dialogue.Contracts;
using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record ShowReplicaStepSnapshot(
    Guid Id,
    ReplicaSnapshot Replica,
    TransitionSnapshot Transition
) : StepSnapshot(Id, Transition);
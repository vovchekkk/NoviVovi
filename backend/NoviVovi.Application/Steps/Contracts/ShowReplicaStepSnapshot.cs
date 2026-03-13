using NoviVovi.Application.Dialogue.Contracts;
using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Steps.Contracts;

public record ShowReplicaStepSnapshot(
    Guid Id,
    ReplicaSnapshot Replica,
    NextStepTransitionSnapshot Transition
) : StepSnapshot<NextStepTransitionSnapshot>(Id, Transition);
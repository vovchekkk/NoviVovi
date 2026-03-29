using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record ShowReplicaStepDto(
    Guid Id,
    ReplicaDto Replica,
    TransitionDto Transition
) : StepDto(Id, Transition);
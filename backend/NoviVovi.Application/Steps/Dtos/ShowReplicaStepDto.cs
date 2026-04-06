using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record ShowReplicaStepDto : StepDto<NextStepTransitionDto>
{
    public required ReplicaDto Replica { get; init; }
}
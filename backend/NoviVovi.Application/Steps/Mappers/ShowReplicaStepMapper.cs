using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Application.Transitions.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Mappers;

public partial class ShowReplicaStepMapper(ReplicaMapper replicaMapper, TransitionMapper transitionMapper)
{
    public ShowReplicaStepSnapshot ToSnapshot(ShowReplicaStep step)
    {
        return new ShowReplicaStepSnapshot(
            step.Id,
            replicaMapper.ToSnapshot(step.Replica),
            transitionMapper.ToSnapshot(step.Transition)
        );
    }
}
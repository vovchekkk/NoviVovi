using NoviVovi.Api.Dialogue.Mappers;
using NoviVovi.Api.Steps.Responses;
using NoviVovi.Api.Transitions.Mappers;
using NoviVovi.Application.Steps.Contracts;

namespace NoviVovi.Api.Steps.Mappers;

public partial class ShowReplicaStepResponseMapper(ReplicaResponseMapper replicaMapper, TransitionResponseMapper transitionMapper)
{
    public ShowReplicaStepResponse ToSnapshot(ShowReplicaStepSnapshot step)
    {
        return new ShowReplicaStepResponse(
            step.Id,
            replicaMapper.ToSnapshot(step.Replica),
            transitionMapper.ToSnapshot(step.Transition)
        );
    }
}
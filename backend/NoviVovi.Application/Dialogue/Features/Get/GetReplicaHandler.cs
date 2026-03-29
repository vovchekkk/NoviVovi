using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Dialogue.Contracts;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Labels;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Dialogue.Features.Get;

public class GetReplicaHandler(
    ILabelRepository labelRepository,
    ReplicaMapper mapper
)
{
    public async Task<ReplicaSnapshot?> Handle(GetReplicaQuery query)
    {
        var label = await labelRepository.GetByIdAsync(query.LabelId);
        if (label == null)
            throw new NotFoundException($"Метка с ID '{query.LabelId}' не найдена");

        if (label.Steps.FirstOrDefault(s => s.Id == query.StepId) is not ShowReplicaStep step)
            throw new NotFoundException($"Шаг с ID '{query.StepId}' не найден");

        var replica = step.Replica;
        if (replica.Id != query.ReplicaId)
            throw new NotFoundException($"Реплика с ID '{query.ReplicaId}' не найдена");

        return mapper.ToSnapshot(replica);
    }
}
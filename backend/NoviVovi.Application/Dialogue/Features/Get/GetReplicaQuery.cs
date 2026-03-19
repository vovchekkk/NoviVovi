namespace NoviVovi.Application.Dialogue.Features.Get;

public record GetReplicaQuery(
    Guid LabelId,
    Guid StepId,
    Guid ReplicaId
);
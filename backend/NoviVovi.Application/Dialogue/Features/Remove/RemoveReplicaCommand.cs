namespace NoviVovi.Application.Dialogue.Features.Remove;

public record RemoveReplicaCommand(
    Guid LabelId,
    Guid Id
);
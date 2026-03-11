namespace NoviVovi.Application.Dialogue.Features.Add;

public record AddReplicaCommand(
    Guid LabelId,
    Guid SpeakerId,
    string Text
);
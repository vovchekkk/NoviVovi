namespace NoviVovi.Application.Dialogue.Features.Add;

public record AddReplicaCommand(
    Guid NovelId,
    Guid LabelId,
    Guid SpeakerId,
    string Text
);
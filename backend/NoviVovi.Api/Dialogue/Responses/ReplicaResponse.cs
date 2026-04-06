namespace NoviVovi.Api.Dialogue.Responses;

public record ReplicaResponse(
    Guid Id,
    Guid? SpeakerId,
    string Text
);
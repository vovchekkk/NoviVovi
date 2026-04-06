using NoviVovi.Application.Characters.Dtos;

namespace NoviVovi.Application.Dialogue.Dtos;

public record ReplicaDto(
    Guid Id,
    Guid? SpeakerId,
    string Text
);
using NoviVovi.Application.Characters.Contracts;

namespace NoviVovi.Application.Dialogue.Contracts;

public record ReplicaSnapshot(
    Guid Id,
    CharacterSnapshot? Speaker,
    string Text
);
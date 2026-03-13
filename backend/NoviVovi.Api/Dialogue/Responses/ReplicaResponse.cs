using NoviVovi.Api.Characters.Responses;

namespace NoviVovi.Api.Dialogue.Responses;

public record ReplicaResponse(
    Guid Id,
    CharacterResponse? Speaker,
    string Text
);
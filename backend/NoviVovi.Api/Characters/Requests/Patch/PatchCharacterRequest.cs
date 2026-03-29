namespace NoviVovi.Api.Characters.Requests.Patch;

public record PatchCharacterRequest(
    Guid NovelId,
    Guid CharacterId,
    string? Name = null,
    string? Description = null
);
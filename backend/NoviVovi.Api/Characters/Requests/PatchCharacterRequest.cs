namespace NoviVovi.Api.Characters.Requests;

public record PatchCharacterRequest(
    string? Name = null,
    string? Description = null
);
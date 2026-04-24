namespace NoviVovi.Api.Characters.Responses;

public record CharacterResponse
(
    Guid Id,
    string Name,
    string NameColor,
    string? Description,
    List<CharacterStateResponse> CharacterStates
);
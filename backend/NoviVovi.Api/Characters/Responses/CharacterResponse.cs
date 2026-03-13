namespace NoviVovi.Api.Characters.Responses;

public record CharacterResponse
(
    Guid Id,
    string Name,
    string? Description,
    List<CharacterStateResponse> CharacterStates
);
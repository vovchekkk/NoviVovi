namespace NoviVovi.Api.Characters.Requests.Get;

public record GetCharacterStateResponse(
    Guid NovelId,
    Guid CharacterId,
    Guid CharacterStateId
);
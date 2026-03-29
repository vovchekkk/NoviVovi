namespace NoviVovi.Api.Characters.Requests.Delete;

public record DeleteCharacterStateRequest(
    Guid NovelId,
    Guid CharacterId
);
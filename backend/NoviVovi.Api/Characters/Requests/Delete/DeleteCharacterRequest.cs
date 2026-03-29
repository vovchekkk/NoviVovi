namespace NoviVovi.Api.Characters.Requests.Delete;

public record DeleteCharacterRequest(
    Guid NovelId,
    Guid CharacterId
);
namespace NoviVovi.Api.Characters.Requests.Get;

public record GetCharacterRequest(
    Guid NovelId,
    Guid CharacterId
);
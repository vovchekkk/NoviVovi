namespace NoviVovi.Api.Characters.Requests.Get;

public record GetCharacterStatesResponse(
    Guid NovelId,
    Guid CharacterId
);
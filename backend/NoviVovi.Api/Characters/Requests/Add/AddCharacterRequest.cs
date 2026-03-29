namespace NoviVovi.Api.Characters.Requests.Add;

public record AddCharacterRequest(
    Guid NovelId,
    string Name,
    string? Description
);
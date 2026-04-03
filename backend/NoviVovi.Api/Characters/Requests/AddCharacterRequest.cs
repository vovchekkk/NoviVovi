namespace NoviVovi.Api.Characters.Requests;

public record AddCharacterRequest(
    string Name,
    string? Description
);
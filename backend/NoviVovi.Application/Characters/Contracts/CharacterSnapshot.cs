namespace NoviVovi.Application.Characters.Contracts;

public record CharacterSnapshot
(
    Guid Id,
    string Name,
    string? Description,
    List<CharacterStateSnapshot> CharacterStates
);
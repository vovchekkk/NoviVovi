using NoviVovi.Application.Characters.Contracts;

namespace NoviVovi.Application.Scene.Contracts;

public record CharacterObjectSnapshot(
    CharacterSnapshot Character,
    CharacterStateSnapshot State
);
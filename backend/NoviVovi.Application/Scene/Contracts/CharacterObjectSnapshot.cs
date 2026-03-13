using NoviVovi.Application.Characters.Contracts;

namespace NoviVovi.Application.Scene.Contracts;

public record CharacterObjectSnapshot(
    Guid Id,
    CharacterSnapshot Character,
    CharacterStateSnapshot State,
    TransformSnapshot Transform
) : SceneObjectSnapshot(Id, Transform);
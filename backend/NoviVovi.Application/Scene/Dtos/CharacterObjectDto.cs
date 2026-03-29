using NoviVovi.Application.Characters.Dtos;

namespace NoviVovi.Application.Scene.Dtos;

public record CharacterObjectDto(
    Guid Id,
    CharacterDto Character,
    CharacterStateDto State,
    TransformDto Transform
) : SceneObjectDto(Id, Transform);
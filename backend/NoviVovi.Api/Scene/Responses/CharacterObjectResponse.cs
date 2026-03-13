using NoviVovi.Api.Characters.Responses;

namespace NoviVovi.Api.Scene.Responses;

public record CharacterObjectResponse(
    Guid Id,
    CharacterResponse Character,
    CharacterStateResponse State,
    TransformResponse Transform
) : SceneObjectResponse(Id, Transform);
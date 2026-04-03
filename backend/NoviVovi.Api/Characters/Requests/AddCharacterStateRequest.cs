using NoviVovi.Api.Scene.Requests;

namespace NoviVovi.Api.Characters.Requests;

public record AddCharacterStateRequest(
    string Name,
    string? Description,
    Guid ImageId,
    TransformRequest? Transform
);
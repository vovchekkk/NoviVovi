using NoviVovi.Api.Scene.Requests;

namespace NoviVovi.Api.Characters.Requests;

public record PatchCharacterStateRequest(
    string? Name,
    string? Description,
    Guid? ImageId,
    TransformRequest? Transform
);
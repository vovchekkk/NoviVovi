using NoviVovi.Api.Images.Responses;
using NoviVovi.Api.Scene.Responses;

namespace NoviVovi.Api.Characters.Responses;

public record CharacterStateResponse(
    Guid Id,
    string Name,
    string? Description,
    ImageResponse Image,
    TransformResponse LocalTransform
);
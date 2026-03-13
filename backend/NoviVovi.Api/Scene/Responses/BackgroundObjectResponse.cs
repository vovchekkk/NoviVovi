using NoviVovi.Api.Images.Responses;

namespace NoviVovi.Api.Scene.Responses;

public record BackgroundObjectResponse(
    Guid Id,
    ImageResponse Image,
    TransformResponse Transform
) : SceneObjectResponse(Id, Transform);
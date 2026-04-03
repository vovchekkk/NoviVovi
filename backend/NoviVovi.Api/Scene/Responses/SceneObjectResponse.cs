namespace NoviVovi.Api.Scene.Responses;

public abstract record SceneObjectResponse(
    Guid Id,
    TransformResponse Transform
);
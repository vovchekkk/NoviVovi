namespace NoviVovi.Application.Scene.Contracts;

public abstract record SceneObjectSnapshot(
    Guid Id,
    TransformSnapshot Transform
);
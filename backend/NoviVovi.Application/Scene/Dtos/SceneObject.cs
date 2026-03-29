namespace NoviVovi.Application.Scene.Dtos;

public abstract record SceneObjectDto(
    Guid Id,
    TransformDto Transform
);
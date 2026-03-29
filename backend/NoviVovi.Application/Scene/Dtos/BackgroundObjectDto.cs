using NoviVovi.Application.Images.Dtos;

namespace NoviVovi.Application.Scene.Dtos;

public record BackgroundObjectDto(
    Guid Id,
    ImageDto Image,
    TransformDto Transform
) : SceneObjectDto(Id, Transform);
using NoviVovi.Application.Images.Contracts;

namespace NoviVovi.Application.Scene.Contracts;

public record BackgroundObjectSnapshot(
    Guid Id,
    ImageSnapshot Image,
    TransformSnapshot Transform
) : SceneObjectSnapshot(Id, Transform);
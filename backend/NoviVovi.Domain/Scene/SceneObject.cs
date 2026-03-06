using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;

namespace NoviVovi.Domain.Scene;

public abstract class SceneObject(Guid id, Transform transform) : Entity(id)
{
    public Transform Transform { get; private set; } = transform;

    public void TransformObject(Transform transform) => Transform = transform;

    public void Move(Position position) => Transform.Position = position;
    public void ChangeSize(Size size) => Transform.Size = size;
    public void ChangeScale(double scale) => Transform.Scale = scale;
    public void ChangeRotation(double rotation) => Transform.Rotation = rotation;
    public void ChangeZIndex(int z) => Transform.ZIndex = z;
}
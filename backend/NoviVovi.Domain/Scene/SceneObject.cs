using System.Drawing;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;

namespace NoviVovi.Domain.Scene;

public abstract class SceneObject(Guid id, Transform transform) : Entity(id)
{
    public Transform Transform { get; private set; } = transform;

    public void PatchTransform(TransformPatch transformPatch)
    {
        Transform = Transform.ApplyPatch(transformPatch);
    }
}
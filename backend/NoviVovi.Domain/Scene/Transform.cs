using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Scene;

public class Transform : ValueObject
{
    public Position Position { get; init; } = new(0.0, 0.0);
    public Size Size { get; init; } = new(0, 0);
    public double Scale { get; init; } = 1.0;
    public double Rotation { get; init; }
    public int ZIndex { get; init; }

    public Transform ApplyPatch(TransformPatch patch)
    {
        return new Transform
        {
            Position = new Position(
                patch.X ?? Position.X,
                patch.Y ?? Position.Y
            ),
            Size = new Size(
                patch.Width ?? Size.Width,
                patch.Height ?? Size.Height
            ),
            Scale = patch.Scale ?? Scale,
            Rotation = patch.Rotation ?? Rotation,
            ZIndex = patch.ZIndex ?? ZIndex
        };
    }
}
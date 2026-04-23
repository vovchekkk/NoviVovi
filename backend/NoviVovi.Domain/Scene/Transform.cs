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

    public static Transform operator +(Transform a, Transform b)
    {
        return new Transform
        {
            Position = new Position(a.Position.X + b.Position.X, a.Position.Y + b.Position.Y),
            Size = new Size(a.Size.Width + b.Size.Width, a.Size.Height + b.Size.Height),
            Scale = a.Scale * b.Scale,
            Rotation = a.Rotation + b.Rotation,
            ZIndex = a.ZIndex + b.ZIndex
        };
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Position;
        yield return Size;
        yield return Scale;
        yield return Rotation;
        yield return ZIndex;
    }
}
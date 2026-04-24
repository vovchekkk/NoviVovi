using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Scene;

public class Transform : Entity
{
    public Position Position { get; init; }
    public Size Size { get; init; }
    public double Scale { get; init; }
    public double Rotation { get; init; }
    public int ZIndex { get; init; }

    private Transform(
        Guid id,
        Position position,
        Size size,
        double scale,
        double rotation,
        int zIndex
    ) : base(id)
    {
        Position = position;
        Size = size;
        Scale = scale;
        Rotation = rotation;
        ZIndex = zIndex;
    }

    public static Transform Create(
        Position? position,
        Size? size,
        double scale = 1.0,
        double rotation = 0.0,
        int zIndex = 0
    )
    {
        if (position is null)
            throw new DomainException("Position cannot be null");

        if (size is null)
            throw new DomainException("Size cannot be null");

        return new Transform(Guid.NewGuid(), position, size, scale, rotation, zIndex);
    }

    public Transform ApplyPatch(TransformPatch patch)
    {
        return new Transform(
            Id,
            new Position(
                patch.X ?? Position.X,
                patch.Y ?? Position.Y
            ),
            new Size(
                patch.Width ?? Size.Width,
                patch.Height ?? Size.Height
            ),
            patch.Scale ?? Scale,
            patch.Rotation ?? Rotation,
            patch.ZIndex ?? ZIndex
        );
    }

    public static Transform operator +(Transform a, Transform b)
    {
        return new Transform(
            Guid.Empty,
            new Position(
                a.Position.X + b.Position.X, 
                a.Position.Y + b.Position.Y
            ),
            new Size(
                a.Size.Width + b.Size.Width, 
                a.Size.Height + b.Size.Height
            ),
            a.Scale * b.Scale,
            a.Rotation + b.Rotation,
            a.ZIndex + b.ZIndex
        );
    }
}
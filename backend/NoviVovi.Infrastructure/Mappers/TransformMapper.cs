using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class TransformMapper
{
    public Transform ToDomain(TransformDbO? dbo)
    {
        dbo ??= new TransformDbO();
        var res = new Transform
        {
            Position = dbo is { YPos: not null, XPos: not null } ? new Position((double)dbo.XPos.Value, (double)dbo.YPos.Value) : new Position(0, 0),
            Rotation = dbo.Rotation == null ? 0 : (double)dbo.Rotation,
            Scale = dbo.Scale==null ? 1 : (double)dbo.Scale,
            ZIndex = dbo.ZIndex ?? 0,
            Size = new Size(dbo.Width, dbo.Height)
        };
        return res;
    }

    public TransformDbO ToDbO(Transform transform)
    {
        return new TransformDbO
        {
            Height = transform.Size.Height,
            Width = transform.Size.Width,
            Rotation = transform.Scale == 0 ? null : (decimal)transform.Rotation,
            Scale = transform.Scale == 0 ? 1 : (decimal)transform.Scale,
            XPos = (decimal)transform.Position.X,
            YPos = (decimal)transform.Position.Y,
            ZIndex = transform.ZIndex
        };
    }
}
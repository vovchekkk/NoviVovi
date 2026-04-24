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
        (
            dbo.Id,
            dbo is { YPos: not null, XPos: not null }
                ? new Position((double)dbo.XPos.Value, (double)dbo.YPos.Value)
                : new Position(),
            new Size(dbo.Width, dbo.Height),
            dbo.Rotation == null ? 0 : (double)dbo.Rotation,
            dbo.Scale == null ? 1 : (double)dbo.Scale,
            dbo.ZIndex ?? 0
        );
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
using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class TransformMapper
{
    public Transform ToDomain(TransformDbO? dbo)
    {
        if (dbo == null)
            dbo = new TransformDbO { Id = Guid.NewGuid() };
            
        var res = new Transform
        (
            dbo.Id,
            dbo is { YPos: not null, XPos: not null }
                ? new Position((double)dbo.XPos.Value, (double)dbo.YPos.Value)
                : new Position(),
            new Size(dbo.Width, dbo.Height),
            dbo.Scale == null ? 1 : (double)dbo.Scale,
            dbo.Rotation == null ? 0 : (double)dbo.Rotation,
            dbo.ZIndex ?? 0
        );
        return res;
    }

    public TransformDbO ToDbO(Transform transform)
    {
        return new TransformDbO
        {
            Id = transform.Id,
            Height = transform.Size.Height,
            Width = transform.Size.Width,
            Rotation = (decimal)transform.Rotation,
            Scale = (decimal)transform.Scale,
            XPos = (decimal)transform.Position.X,
            YPos = (decimal)transform.Position.Y,
            ZIndex = transform.ZIndex
        };
    }
}
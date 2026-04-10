using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class TransformMapper
{
    public Transform ToDomain(TransformDbO dbo)
    {
        var res = new Transform();
        if (dbo is { YPos: not null, XPos: not null }) 
            res.Position = new Position((double)dbo.XPos.Value, (double)dbo.YPos.Value);
        res.Rotation = dbo.Rotation == null ? 0 : (double)dbo.Rotation;
        res.Scale = dbo.Scale==null ? 1 : (double)dbo.Scale;
        res.ZIndex = dbo.ZIndex ?? 0;
        res.Size = new Size(dbo.Width, dbo.Height);
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
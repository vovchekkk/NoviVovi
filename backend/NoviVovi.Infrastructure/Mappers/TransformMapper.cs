using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class TransformMapper
{
    public Transform ToTransform(TransformDbO dbo)
    {
        var res = new Transform();
        if (dbo is { YPos: not null, XPos: not null }) 
            res.Position = new Position((double)dbo.XPos.Value, (double)dbo.YPos.Value);
        res.Rotation = dbo.Rotation==null ? 0 : (double)dbo.Rotation;
        res.Scale = dbo.Scale==null ? 1 : (double)dbo.Scale;
        res.ZIndex = dbo.ZIndex ?? 0;
        res.Size = new Size(dbo.Width, dbo.Height);
        return res;
    }
}
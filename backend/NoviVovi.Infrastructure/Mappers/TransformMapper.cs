using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class TransformMapper
{
    public Transform ToTransform(TransformDbO dbo) => new()
    {
        Position = dbo.XPos.HasValue && dbo.YPos.HasValue
            ? new Position((double)dbo.XPos.Value, (double)dbo.YPos.Value)
            : new Position(0.0, 0.0),

        Size = new Size(dbo.Width, dbo.Height),

        Scale = dbo.Scale.HasValue ? (double)dbo.Scale : 1.0,
        Rotation = dbo.Rotation.HasValue ? (double)dbo.Rotation : 0.0,
        ZIndex = dbo.ZIndex ?? 0
    };
}
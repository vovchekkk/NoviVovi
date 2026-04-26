using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Mappers;

public class TransformToRenPyMapper
{
    /// <summary>
    /// Maps Transform to RenPyTransform without image size information.
    /// XZoom and YZoom will be set to 1.0 (no scaling).
    /// </summary>
    public RenPyTransform Map(Transform transform)
    {
        return new RenPyTransform(
            transform.Position.X,
            transform.Position.Y,
            transform.Scale,
            XZoom: 1.0,
            YZoom: 1.0,
            transform.Rotation,
            transform.ZIndex
        );
    }

    /// <summary>
    /// Maps Transform to RenPyTransform with image size information.
    /// Calculates XZoom = TransformWidth / ImageWidth and YZoom = TransformHeight / ImageHeight.
    /// </summary>
    public RenPyTransform Map(Transform transform, Size imageSize)
    {
        var xZoom = imageSize.Width > 0 ? (double)transform.Size.Width / imageSize.Width : 1.0;
        var yZoom = imageSize.Height > 0 ? (double)transform.Size.Height / imageSize.Height : 1.0;

        return new RenPyTransform(
            transform.Position.X,
            transform.Position.Y,
            transform.Scale,
            xZoom,
            yZoom,
            transform.Rotation,
            transform.ZIndex
        );
    }
}
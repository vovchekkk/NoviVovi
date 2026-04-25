using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Mappers;

public class TransformToRenPyMapper
{
    public RenPyTransform Map(Transform transform)
    {
        return new RenPyTransform(
            (int)transform.Position.X,
            (int)transform.Position.Y,
            transform.Scale,
            transform.Rotation,
            transform.ZIndex
        );
    }
}
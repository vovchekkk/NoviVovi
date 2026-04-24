using NoviVovi.Domain.Scene;
using NoviVovi.Infrastructure.Exporters.RenPy.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Mappers;

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
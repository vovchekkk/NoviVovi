using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class TransformDtoMapper
{
    public TransformDto ToDto(Transform s) => new()
    {
        X = s.Position.X,
        Y = s.Position.Y,
        Width = s.Size.Width,
        Height = s.Size.Height,
        Scale = s.Scale,
        Rotation = s.Rotation,
        ZIndex = s.ZIndex
    };

    public partial IEnumerable<TransformDto> ToDtos(IEnumerable<Transform> subjects);
}
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class TransformDtoMapper
{
    public TransformDto ToDto(Transform source) => new()
    {
        X = source.Position.X,
        Y = source.Position.Y,
        Width = source.Size.Width,
        Height = source.Size.Height,
        Scale = source.Scale,
        Rotation = source.Rotation,
        ZIndex = source.ZIndex
    };

    public partial IEnumerable<TransformDto> ToDtos(IEnumerable<Transform> sources);
    
    public Transform ToDomainModel(TransformDto source) => new()
    {
        Position = new Position(source.X, source.Y),
        Size = new Size(source.Width, source.Height),
        Scale = source.Scale,
        Rotation = source.Rotation,
        ZIndex = source.ZIndex
    };
    
    public partial IEnumerable<Transform> ToDomainModels(IEnumerable<TransformDto> sources);
    
    public partial TransformPatch ToDomainPatch(TransformDto source);
    
    public partial IEnumerable<TransformPatch> ToDomainPatches(IEnumerable<TransformDto> sources);
}
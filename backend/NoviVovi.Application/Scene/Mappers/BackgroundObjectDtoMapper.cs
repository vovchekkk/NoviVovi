using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class BackgroundObjectDtoMapper(
    TransformDtoMapper transformMapper
)
{
    public partial BackgroundObjectDto ToDto(BackgroundObject subject);

    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);

    public partial IEnumerable<BackgroundObjectDto> ToDtos(IEnumerable<BackgroundObject> subjects);
}
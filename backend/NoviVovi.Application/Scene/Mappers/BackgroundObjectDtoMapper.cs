using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class BackgroundObjectDtoMapper(
    ImageDtoMapper imageMapper,
    TransformDtoMapper transformMapper
)
{
    public partial BackgroundObjectDto ToDto(BackgroundObject source);

    private ImageDto MapImage(Image source) => imageMapper.ToDto(source);
    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);

    public partial IEnumerable<BackgroundObjectDto> ToDtos(IEnumerable<BackgroundObject> sources);
}
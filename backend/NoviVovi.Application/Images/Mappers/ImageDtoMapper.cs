using NoviVovi.Application.Images.Dtos;
using NoviVovi.Domain.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Images.Mappers;

[Mapper]
public partial class ImageDtoMapper
{
    public partial ImageDto ToDto(Image subject);

    public partial IEnumerable<ImageDto> ToDtos(IEnumerable<Image> subjects);
}
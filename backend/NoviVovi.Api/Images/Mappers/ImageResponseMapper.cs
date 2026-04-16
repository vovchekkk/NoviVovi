using NoviVovi.Api.Images.Responses;
using NoviVovi.Application.Images.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Images.Mappers;

[Mapper]
public partial class ImageResponseMapper
{
    public partial ImageResponse ToResponse(ImageDto source);

    public partial IEnumerable<ImageResponse> ToResponses(IEnumerable<ImageDto> sources);
}
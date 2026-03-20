using NoviVovi.Api.Images.Responses;
using NoviVovi.Application.Images.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Images.Mappers;

[Mapper]
public partial class ImageResponseMapper
{
    public partial ImageResponse ToResponse(ImageSnapshot subject);

    public partial IEnumerable<ImageResponse> ToResponses(IEnumerable<ImageSnapshot> subjects);
}
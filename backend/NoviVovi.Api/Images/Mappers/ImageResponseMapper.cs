using NoviVovi.Api.Images.Responses;
using NoviVovi.Application.Images.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Images.Mappers;

[Mapper]
public partial class ImageResponseMapper
{
    public partial ImageResponse ToSnapshot(ImageSnapshot novel);

    public partial IEnumerable<ImageResponse> ToSnapshots(IEnumerable<ImageSnapshot> novels);
}
using NoviVovi.Application.Images.Contracts;
using NoviVovi.Domain.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Images.Mappers;

[Mapper]
public partial class ImageMapper
{
    public partial ImageSnapshot ToSnapshot(Image subject);

    public partial IEnumerable<ImageSnapshot> ToSnapshots(IEnumerable<Image> subjects);
}
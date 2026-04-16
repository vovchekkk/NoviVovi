using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Images.Mappers;

[Mapper]
public partial class UploadInfoImageDtoMapper
{
    public partial UploadInfoImageDto ToDto(UploadInfoImage subject);

    public partial IEnumerable<UploadInfoImageDto> ToDtos(IEnumerable<UploadInfoImage> subjects);
}
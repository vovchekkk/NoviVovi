using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Domain.Images;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Images.Mappers;

[Mapper]
public partial class ImageDtoMapper(
    IStorageService storageService,
    SizeDtoMapper sizeMapper
)
{
    public ImageDto ToDto(Image source) => new()
    {
        Id = source.Id,
        Name = source.Name,
        Description = source.Description,
        Url = storageService.GetViewUrl(source.StoragePath), 
        StoragePath = source.StoragePath,
        Format = source.Format,
        Size = sizeMapper.ToDto(source.Size),
        Type = MapType(source.Type),
        Status = MapStatus(source.Status)
    };
    
    private partial ImageTypeDto MapType(ImageType source);
    private partial ImageStatusDto MapStatus(ImageStatus source);
    
    public partial IEnumerable<ImageDto> ToDtos(IEnumerable<Image> sources);
}
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class SizeDtoMapper
{
    public partial SizeDto ToDto(Size subject);
    
    public partial IEnumerable<SizeDto> ToDtos(IEnumerable<Size> subjects);
}
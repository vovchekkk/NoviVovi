using NoviVovi.Application.Scene.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class SizeDtoMapper
{
    public partial SizeDto ToDto(SizeDto subject);
    
    public partial IEnumerable<SizeDto> ToDtos(IEnumerable<SizeDto> subjects);
}
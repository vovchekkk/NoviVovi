using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class BackgroundObjectDtoMapper
{
    public partial BackgroundObjectDto ToDto(BackgroundObject subject);
    
    public partial IEnumerable<BackgroundObjectDto> ToDtos(IEnumerable<BackgroundObject> subjects);
}
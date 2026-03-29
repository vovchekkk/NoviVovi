using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class CharacterObjectDtoMapper
{
    public partial CharacterObjectDto ToDto(CharacterObject subject);
    
    public partial IEnumerable<CharacterObjectDto> ToDtos(IEnumerable<CharacterObject> subjects);
}
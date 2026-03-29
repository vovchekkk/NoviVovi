using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Domain.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Characters.Mappers;

[Mapper]
public partial class CharacterDtoMapper
{
    public partial CharacterDto ToDto(Character subject);
    
    public partial IEnumerable<CharacterDto> ToDtos(IEnumerable<Character> subjects);
}
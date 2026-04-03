using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Domain.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Characters.Mappers;

[Mapper]
public partial class CharacterDtoMapper(
    CharacterStateDtoMapper characterStateMapper
)
{
    public partial CharacterDto ToDto(Character subject);

    private CharacterStateDto MapCharacterState(CharacterState source) => characterStateMapper.ToDto(source);

    public partial IEnumerable<CharacterDto> ToDtos(IEnumerable<Character> subjects);
}
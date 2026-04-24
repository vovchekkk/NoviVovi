using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Characters.Mappers;

[Mapper]
public partial class CharacterDtoMapper(
    CharacterStateDtoMapper characterStateMapper
)
{
    public partial CharacterDto ToDto(Character source);

    private CharacterStateDto MapCharacterState(CharacterState source) => characterStateMapper.ToDto(source);
    
    private string MapColor(Color color) => color.Value;

    public partial IEnumerable<CharacterDto> ToDtos(IEnumerable<Character> sources);
}
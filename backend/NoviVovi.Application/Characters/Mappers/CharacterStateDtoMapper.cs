using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Domain.Characters;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Characters.Mappers;

[Mapper]
public partial class CharacterStateDtoMapper
{
    public partial CharacterStateDto ToDto(CharacterState subject);

    public partial IEnumerable<CharacterStateDto> ToDtos(IEnumerable<CharacterState> subjects);
}
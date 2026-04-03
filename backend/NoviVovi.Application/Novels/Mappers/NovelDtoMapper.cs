using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Novels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NovelDtoMapper(
    CharacterDtoMapper characterMapper
)
{
    public partial NovelDto ToDto(Novel subject);

    private CharacterDto MapCharacter(Character source) => characterMapper.ToDto(source);

    public partial IEnumerable<NovelDto> ToDtos(IEnumerable<Novel> subjects);
}
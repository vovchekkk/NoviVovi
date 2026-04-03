using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Scene.Mappers;

[Mapper]
public partial class CharacterObjectResponseMapper(
    CharacterDtoMapper characterMapper,
    TransformDtoMapper transformMapper
)
{
    public partial CharacterObjectDto ToResponse(CharacterObject subject);

    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);
    private CharacterDto MapCharacter(Character source) => characterMapper.ToDto(source);

    public partial IEnumerable<CharacterObjectDto> ToResponses(IEnumerable<CharacterObject> subjects);
}
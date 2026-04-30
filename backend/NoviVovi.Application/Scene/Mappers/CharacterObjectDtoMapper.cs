using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class CharacterObjectDtoMapper(
    CharacterDtoMapper characterMapper,
    CharacterStateDtoMapper characterStateMapper,
    TransformDtoMapper transformMapper
)
{
    public CharacterObjectDto ToDto(CharacterObject source)
    {
        return new CharacterObjectDto(
            source.Id,
            characterMapper.ToDto(source.Character),
            characterStateMapper.ToDto(source.State),
            transformMapper.ToDto(source.Transform)
        );
    }

    public partial IEnumerable<CharacterObjectDto> ToDtos(IEnumerable<CharacterObject> sources);
}
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Scene.Mappers;

[Mapper]
public partial class CharacterObjectDtoMapper(
    TransformDtoMapper transformMapper
)
{
    public partial CharacterObjectDto ToDto(CharacterObject subject);

    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);

    public partial IEnumerable<CharacterObjectDto> ToDtos(IEnumerable<CharacterObject> subjects);
}
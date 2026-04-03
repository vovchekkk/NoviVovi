using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Characters.Mappers;

[Mapper]
public partial class CharacterStateDtoMapper(
    TransformDtoMapper transformMapper
)
{
    public partial CharacterStateDto ToDto(CharacterState subject);

    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);

    public partial IEnumerable<CharacterStateDto> ToDtos(IEnumerable<CharacterState> subjects);
}
using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Characters.Mappers;

[Mapper]
public partial class CharacterStateDtoMapper(
    ImageDtoMapper imageMapper,
    TransformDtoMapper transformMapper
)
{
    public partial CharacterStateDto ToDto(CharacterState source);

    private ImageDto MapImage(Image source) => imageMapper.ToDto(source);
    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);

    public partial IEnumerable<CharacterStateDto> ToDtos(IEnumerable<CharacterState> sources);
}
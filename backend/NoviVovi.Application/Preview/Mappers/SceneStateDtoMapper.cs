using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Dialogue.Dtos;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Preview.Dtos;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Preview;
using NoviVovi.Domain.Scene;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Preview.Mappers;

[Mapper]
public partial class SceneStateDtoMapper(
    ReplicaDtoMapper replicaMapper,
    CharacterDtoMapper characterMapper,
    ImageDtoMapper imageMapper,
    TransformDtoMapper transformMapper
)
{
    public partial SceneStateDto ToDto(VisualSnapshot source);

    private ImageDto MapImage(Image source) => imageMapper.ToDto(source);
    private TransformDto MapTransform(Transform source) => transformMapper.ToDto(source);
    private ReplicaDto MapReplica(Replica source) => replicaMapper.ToDto(source);
    private CharacterDto MapCharacter(Character source) => characterMapper.ToDto(source);

    public partial IEnumerable<SceneStateDto> ToDtos(IEnumerable<VisualSnapshot> source);
}
using NoviVovi.Application.Preview.Dtos;
using NoviVovi.Application.Preview.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Preview.Mappers;

[Mapper]
public partial class SceneStateDtoMapper
{
    public partial SceneStateDto ToDto(SceneState subject);

    public partial IEnumerable<SceneStateDto> ToDtos(IEnumerable<SceneState> subjects);
}
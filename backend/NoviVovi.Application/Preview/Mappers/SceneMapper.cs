using NoviVovi.Application.Preview.Contracts;
using NoviVovi.Application.Preview.Models;
using NoviVovi.Application.Preview.Services;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Preview.Mappers;

[Mapper]
public partial class SceneMapper
{
    public partial SceneSnapshot ToSnapshot(SceneState novel);

    public partial IEnumerable<SceneSnapshot> ToSnapshots(IEnumerable<SceneState> novels);
}
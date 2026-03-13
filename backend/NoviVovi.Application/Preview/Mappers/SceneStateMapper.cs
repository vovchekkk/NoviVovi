using NoviVovi.Application.Preview.Contracts;
using NoviVovi.Application.Preview.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Preview.Mappers;

[Mapper]
public partial class SceneStateMapper
{
    public partial SceneStateSnapshot ToSnapshot(SceneState novel);

    public partial IEnumerable<SceneStateSnapshot> ToSnapshots(IEnumerable<SceneState> novels);
}
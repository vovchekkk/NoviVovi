using NoviVovi.Application.Preview.Contracts;
using NoviVovi.Application.Preview.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Preview.Mappers;

[Mapper]
public partial class SceneStateSnapshotMapper
{
    public partial SceneStateSnapshot ToSnapshot(SceneState subject);

    public partial IEnumerable<SceneStateSnapshot> ToSnapshots(IEnumerable<SceneState> subjects);
}
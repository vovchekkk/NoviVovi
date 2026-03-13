using NoviVovi.Api.Preview.Responses;
using NoviVovi.Application.Preview.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Preview.Mappers;

[Mapper]
public partial class SceneStateResponseMapper
{
    public partial SceneStateResponse ToSnapshot(SceneStateSnapshot novel);

    public partial IEnumerable<SceneStateResponse> ToSnapshots(IEnumerable<SceneStateSnapshot> novels);
}
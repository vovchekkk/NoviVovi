using NoviVovi.Api.Preview.Responses;
using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Preview.Contracts;
using NoviVovi.Application.Transitions.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Preview.Mappers;

[Mapper]
public partial class SceneStateResponseMapper
{
    public partial SceneStateResponse ToResponse(SceneStateSnapshot novel);

    public partial IEnumerable<SceneStateResponse> ToResponses(IEnumerable<SceneStateSnapshot> novels);
}
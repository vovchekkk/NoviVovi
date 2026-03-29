using NoviVovi.Api.Preview.Responses;
using NoviVovi.Api.Transitions.Responses;
using NoviVovi.Application.Preview.Dtos;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Preview.Mappers;

[Mapper]
public partial class SceneStateResponseMapper
{
    public partial SceneStateResponse ToResponse(SceneStateDto subject);

    public partial IEnumerable<SceneStateResponse> ToResponses(IEnumerable<SceneStateDto> subjects);
}
using NoviVovi.Api.Novels.Responses;
using NoviVovi.Api.Novels.Responses.Edges;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Dtos.Edges;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class NovelGraphResponseMapper(
    EdgeResponseMapper edgeMapper
)
{
    public partial NovelGraphResponse ToResponse(NovelGraphDto source);

    private EdgeResponse MapEdge(EdgeDto source) => edgeMapper.ToResponse(source);

    public partial IEnumerable<NovelGraphResponse> ToResponses(IEnumerable<NovelGraphDto> sources);
}
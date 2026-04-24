using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Dtos.Edges;
using NoviVovi.Application.Novels.Dtos.Nodes;
using NoviVovi.Application.Novels.Models;
using NoviVovi.Application.Novels.Models.Edges;
using NoviVovi.Application.Novels.Models.Nodes;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NovelGraphDtoMapper(
    EdgeDtoMapper edgeMapper,
    NodeDtoMapper nodeMapper
)
{
    [MapProperty(nameof(NovelGraph.Nodes), nameof(NovelGraphDto.Nodes))]
    [MapProperty(nameof(NovelGraph.Edges), nameof(NovelGraphDto.Edges))]
    public partial NovelGraphDto ToDto(NovelGraph source);

    private EdgeDto MapEdge(Edge source) => edgeMapper.ToDto(source);
    
    private NodeDto MapNode(Node source) => nodeMapper.ToDto(source);

    public partial IEnumerable<NovelGraphDto> ToDtos(IEnumerable<NovelGraph> sources);
}
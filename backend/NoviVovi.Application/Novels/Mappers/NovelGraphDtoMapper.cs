using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Models;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NovelGraphDtoMapper(
    EdgeDtoMapper edgeMapper
)
{
    public partial NovelGraphDto ToDto(NovelGraph subject);

    private EdgeDto MapEdge(Edge source) => edgeMapper.ToDto(source);

    public partial IEnumerable<NovelGraphDto> ToDtos(IEnumerable<NovelGraph> subjects);
}
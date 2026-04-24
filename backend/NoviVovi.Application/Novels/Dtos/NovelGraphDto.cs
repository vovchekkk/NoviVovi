using NoviVovi.Application.Novels.Dtos.Edges;
using NoviVovi.Application.Novels.Dtos.Nodes;

namespace NoviVovi.Application.Novels.Dtos;

public record NovelGraphDto(
    List<NodeDto> Nodes,
    List<EdgeDto> Edges
);
namespace NoviVovi.Application.Novels.Dtos;

public record NovelGraphDto(
    List<NodeDto> Nodes,
    List<EdgeDto> Edges
);
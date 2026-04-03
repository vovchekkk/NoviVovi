namespace NoviVovi.Api.Novels.Responses;

public record NovelGraphResponse(
    List<NodeResponse> Nodes,
    List<EdgeResponse> Edges
);
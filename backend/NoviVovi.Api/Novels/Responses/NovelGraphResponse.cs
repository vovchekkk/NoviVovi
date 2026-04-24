using NoviVovi.Api.Novels.Responses.Edges;
using NoviVovi.Api.Novels.Responses.Nodes;

namespace NoviVovi.Api.Novels.Responses;

public record NovelGraphResponse
{
    public required List<NodeResponse> Nodes { get; init; }
    public required List<EdgeResponse> Edges { get; init; }
}
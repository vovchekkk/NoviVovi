namespace NoviVovi.Application.Novels.Models;

public class NovelGraph
{
    public required List<Node> Nodes { get; init; }
    public required List<Edge> Edges { get; init; }
}
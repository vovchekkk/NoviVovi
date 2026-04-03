using NoviVovi.Application.Novels.Dtos;

namespace NoviVovi.Application.Novels.Models;

public record NovelGraph
{
    public required List<NodeDto> Nodes { get; init; }
    public required List<EdgeDto> Edges { get; init; }
}
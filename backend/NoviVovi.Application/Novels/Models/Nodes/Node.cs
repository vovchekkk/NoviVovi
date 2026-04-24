namespace NoviVovi.Application.Novels.Models.Nodes;

/// <summary>
/// Base class for all node types in the novel graph.
/// </summary>
public class Node
{
    public required Guid LabelId { get; init; }
    public required string LabelName { get; init; }
}
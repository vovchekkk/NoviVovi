namespace NoviVovi.Application.Novels.Models.Edges;

/// <summary>
/// Edge representing a transition from a specific choice to a target label.
/// </summary>
public class ChoiceEdge : Edge
{
    public required Guid SourceChoiceId { get; init; }
}
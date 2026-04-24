namespace NoviVovi.Application.Novels.Models.Edges;

public abstract class Edge
{
    public required Guid StepId { get; init; }
    public required Guid SourceLabelId { get; init; }
    public required Guid TargetLabelId { get; init; }
}
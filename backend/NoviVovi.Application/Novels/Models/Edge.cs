namespace NoviVovi.Application.Novels.Models;

public class Edge
{
    public required Guid Id { get; init; }
    public required Guid SourceLabelId { get; init; }
    public required Guid TargetLabelId { get; init; }
}
using NoviVovi.Application.Labels.Dtos;

namespace NoviVovi.Application.Novels.Models;

public class Edge
{
    public required Guid Id { get; init; }
    public required LabelDto SourceLabel { get; init; }
    public required LabelDto TargetLabel { get; init; }
}
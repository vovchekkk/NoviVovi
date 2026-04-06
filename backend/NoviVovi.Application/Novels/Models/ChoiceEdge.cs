using NoviVovi.Domain.Menu;

namespace NoviVovi.Application.Novels.Models;

public class ChoiceEdge : Edge
{
    public required Choice Choice { get; init; }
    public required string Text { get; init; }
}
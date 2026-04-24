using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Domain.Menu;

namespace NoviVovi.Application.Novels.Models.Nodes;

/// <summary>
/// Node representing a label with menu choices.
/// </summary>
public class MenuNode : Node
{
    public required IReadOnlyCollection<Choice> Choices { get; init; }
}

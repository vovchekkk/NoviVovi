using NoviVovi.Application.Menu.Dtos;

namespace NoviVovi.Application.Novels.Models;

public class ChoiceEdge : Edge
{
    public required ChoiceDto Choice { get; init; }
    public required string Text { get; init; }
}
namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Menu.Models;

/// <summary>
/// Single menu choice with target label for jump
/// </summary>
public record RenPyChoice(
    string Text,
    string TargetLabel
);
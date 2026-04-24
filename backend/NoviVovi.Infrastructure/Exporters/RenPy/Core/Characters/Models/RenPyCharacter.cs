namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Characters.Models;

/// <summary>
/// Represents a Ren'Py character definition.
/// </summary>
public class RenPyCharacter
{
    public required string VariableName { get; init; }  // e.g., "char_a1b2c3..."
    public required string DisplayName { get; init; }   // e.g., "Эйлин"
    public required string Color { get; init; }         // e.g., "#FF5733"
}

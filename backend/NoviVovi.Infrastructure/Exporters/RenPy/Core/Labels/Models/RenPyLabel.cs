using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Labels.Models;

/// <summary>
/// Represents a Ren'Py label with statements.
/// </summary>
public class RenPyLabel
{
    public required string Identifier { get; init; }           // e.g., "label_a1b2c3..."
    public required List<RenPyStatement> Statements { get; init; }
}

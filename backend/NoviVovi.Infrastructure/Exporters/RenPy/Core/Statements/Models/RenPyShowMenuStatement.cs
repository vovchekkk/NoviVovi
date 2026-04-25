using NoviVovi.Infrastructure.Exporters.RenPy.Core.Menu.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;

/// <summary>
/// Represents a Ren'Py menu statement with choices
/// </summary>
public record RenPyShowMenuStatement(
    List<RenPyChoice> Choices
) : RenPyStatement;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Abstractions;

/// <summary>
/// Renders RenPy statements to string representation.
/// Single Responsibility: Statement rendering logic.
/// </summary>
public interface IRenPyStatementRenderer
{
    string Render(RenPyStatement statement, int indentLevel = 1);
}

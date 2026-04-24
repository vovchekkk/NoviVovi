using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Menu.Models;

/// <summary>
/// Menu with choices
/// </summary>
public record RenPyMenu(
    List<RenPyChoice> Choices
) : RenPyStatement;
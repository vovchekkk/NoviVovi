namespace NoviVovi.Infrastructure.Exporters.RenPy.Models;

/// <summary>
/// Menu with choices
/// </summary>
public record RenPyMenu(
    List<RenPyChoice> Choices
) : RenPyStatement;
namespace NoviVovi.Infrastructure.Exporters.RenPy.Models;

/// <summary>
/// Hide character sprite: hide sprite_id
/// </summary>
public record RenPyHideCharacterStatement(
    string ImageName
) : RenPyStatement;
namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;

/// <summary>
/// Hide character sprite: hide sprite_id
/// </summary>
public record RenPyHideCharacterStatement(
    string CharacterName
) : RenPyStatement;
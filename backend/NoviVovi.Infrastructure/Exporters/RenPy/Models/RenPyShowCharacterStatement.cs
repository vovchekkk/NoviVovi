namespace NoviVovi.Infrastructure.Exporters.RenPy.Models;

/// <summary>
/// Show character sprite: show sprite_id at position
/// </summary>
public record RenPyShowCharacterStatement(
    string CharacterName,
    string CharacterStateName,
    RenPyTransform? Transform = null
) : RenPyStatement;

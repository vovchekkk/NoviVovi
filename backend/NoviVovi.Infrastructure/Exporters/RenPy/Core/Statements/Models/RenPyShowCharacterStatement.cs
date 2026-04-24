using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;

/// <summary>
/// Show character sprite: show sprite_id at position
/// </summary>
public record RenPyShowCharacterStatement(
    string CharacterName,
    string CharacterStateName,
    RenPyTransform? Transform = null
) : RenPyStatement;

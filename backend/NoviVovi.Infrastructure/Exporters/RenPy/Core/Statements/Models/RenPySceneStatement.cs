namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;

/// <summary>
/// Show background: scene bg_id
/// </summary>
public record RenPySceneStatement(
    string BackgroundName
) : RenPyStatement;
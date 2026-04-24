namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;

/// <summary>
/// Character dialogue: char "Text"
/// </summary>
public record RenPyReplicaStatement(
    string CharacterVar,
    string Text
) : RenPyStatement;
namespace NoviVovi.Infrastructure.Exporters.RenPy.Models;

/// <summary>
/// Jump to another label: jump target_label
/// </summary>
public record RenPyJumpStatement(
    string TargetLabel
) : RenPyStatement;
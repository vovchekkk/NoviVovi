namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;

/// <summary>
/// Represents Ren'Py transform properties for positioning and transforming displayables.
/// </summary>
public record RenPyTransform(
    int XPos,
    int YPos,
    double Zoom,
    double Rotate,
    int ZOrder
);

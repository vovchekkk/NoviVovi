namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;

/// <summary>
/// Represents Ren'Py transform properties for positioning and transforming displayables.
/// XPos and YPos support both absolute pixels (e.g., 200) and relative coordinates (e.g., 0.5 for 50%).
/// Ren'Py interprets values < 1.0 as relative, >= 1.0 as absolute pixels.
/// Zoom applies uniform scaling, while XZoom and YZoom allow independent axis scaling.
/// </summary>
public record RenPyTransform(
    double XPos,
    double YPos,
    double Zoom,
    double XZoom,
    double YZoom,
    double Rotate,
    int ZOrder
);

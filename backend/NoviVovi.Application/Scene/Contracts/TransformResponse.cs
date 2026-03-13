using System.Drawing;

namespace NoviVovi.Application.Scene.Contracts;

public record TransformSnapshot
(
    PositionSnapshot Position,
    Size Size,
    double Scale,
    double Rotation,
    int ZIndex
);
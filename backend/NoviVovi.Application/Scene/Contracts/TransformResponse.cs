using System.Drawing;

namespace NoviVovi.Application.Scene.Contracts;

public class TransformSnapshot
(
    PositionSnapshot Position,
    Size Size,
    double Scale,
    double Rotation,
    int ZIndex
);
using System.Drawing;

namespace NoviVovi.Api.Scene.Responses;

public class TransformResponse
(
    PositionResponse Position,
    Size Size,
    double Scale,
    double Rotation,
    int ZIndex
);
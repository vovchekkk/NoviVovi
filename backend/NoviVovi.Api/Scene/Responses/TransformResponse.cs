using System.Drawing;

namespace NoviVovi.Api.Scene.Responses;

public record TransformResponse
(
    PositionResponse Position,
    Size Size,
    double Scale,
    double Rotation,
    int ZIndex
);
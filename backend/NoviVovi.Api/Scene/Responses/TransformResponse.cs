namespace NoviVovi.Api.Scene.Responses;

public record TransformResponse
(
    double X,
    double Y,
    int Width,
    int Height,
    double Scale,
    double Rotation,
    int ZIndex
);
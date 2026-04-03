namespace NoviVovi.Api.Scene.Requests;

public record TransformRequest(
    double X,
    double Y,
    int Width,
    int Height,
    double Scale,
    double Rotation,
    int ZIndex
);
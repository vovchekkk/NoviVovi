namespace NoviVovi.Application.Common.Dtos;

public record TransformPatchRequest(
    double X,
    double Y,
    int Width,
    int Height,
    double Scale,
    double Rotation,
    int ZIndex
);
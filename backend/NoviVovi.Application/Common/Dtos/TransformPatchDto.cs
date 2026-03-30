namespace NoviVovi.Application.Common.Dtos;

public record TransformPatchDto(
    double X,
    double Y,
    int Width,
    int Height,
    double Scale,
    double Rotation,
    int ZIndex
);
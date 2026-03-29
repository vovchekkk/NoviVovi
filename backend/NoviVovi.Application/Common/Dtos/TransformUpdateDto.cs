namespace NoviVovi.Application.Common.Dtos;

public record TransformUpdateDto(
    double X,
    double Y,
    int Width,
    int Height,
    double Scale,
    double Rotation,
    int ZIndex
);
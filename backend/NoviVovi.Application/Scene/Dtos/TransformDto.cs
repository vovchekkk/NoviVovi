namespace NoviVovi.Application.Scene.Dtos;

public record TransformDto
(
    PositionDto Position,
    SizeDto Size,
    double Scale,
    double Rotation,
    int ZIndex
);
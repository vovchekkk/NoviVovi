namespace NoviVovi.Application.Scene.Dtos;

public record TransformDto
{
    public required double X { get; init; }
    public required double Y { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required double Scale { get; init; }
    public required double Rotation { get; init; }
    public required int ZIndex { get; init; }
}
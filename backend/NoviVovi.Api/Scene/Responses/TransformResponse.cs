namespace NoviVovi.Api.Scene.Responses;

public record TransformResponse
(
    PositionResponse Position,
    SizeResponse Size,
    double Scale,
    double Rotation,
    int ZIndex
);
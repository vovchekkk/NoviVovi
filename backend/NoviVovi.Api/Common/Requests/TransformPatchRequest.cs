namespace NoviVovi.Api.Common.Requests;

public record TransformPatchRequest(
    double X,
    double Y,
    int Width,
    int Height,
    double Scale,
    double Rotation,
    int ZIndex
);
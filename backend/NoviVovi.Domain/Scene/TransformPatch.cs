namespace NoviVovi.Domain.Scene;

public record TransformPatch(
    double? X = null,
    double? Y = null,
    int? Width = null,
    int? Height = null,
    double? Scale = null,
    double? Rotation = null,
    int? ZIndex = null
);
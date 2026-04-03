namespace NoviVovi.Api.Images.Requests;

public record PatchImageRequest(
    string? Name,
    string? Description,
    string? Format,
    string? Type,
    int? Width,
    int? Height
);
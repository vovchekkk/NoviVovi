namespace NoviVovi.Api.Images.Responses;

public record ImageResponse(
    Guid Id,
    string Name,
    string? Description,
    string Url,
    string Format,
    string Type,
    int Width,
    int Height
);
using NoviVovi.Api.Scene.Responses;

namespace NoviVovi.Api.Images.Responses;

public record ImageResponse(
    Guid Id,
    string Name,
    string? Description,
    string? Url,
    string StoragePath,
    string Format,
    ImageTypeResponse Type,
    SizeResponse Size,
    ImageStatusResponse Status
);
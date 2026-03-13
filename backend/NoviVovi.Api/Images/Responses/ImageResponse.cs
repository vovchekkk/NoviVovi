namespace NoviVovi.Api.Images.Responses;

public record ImageResponse(
    Guid Id,
    string Name,
    string Url,
    string? Description
);
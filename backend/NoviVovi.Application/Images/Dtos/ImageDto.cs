namespace NoviVovi.Application.Images.Dtos;

public record ImageDto(
    Guid Id,
    string Name,
    string Url,
    string? Description
);
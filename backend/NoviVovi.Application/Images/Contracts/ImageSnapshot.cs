namespace NoviVovi.Application.Images.Contracts;

public record ImageSnapshot(
    Guid Id,
    string Name,
    string Url,
    string? Description
);
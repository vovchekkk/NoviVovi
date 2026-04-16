using NoviVovi.Application.Scene.Dtos;

namespace NoviVovi.Application.Images.Dtos;

public record ImageDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string Url { get; init; }
    public required string StoragePath { get; init; }
    public required string Format { get; init; }
    public required ImageTypeDto Type { get; init; }
    public required SizeDto Size { get; init; }
    public required ImageStatusDto Status { get; init; }
}
using NoviVovi.Application.Images.Dtos;

namespace NoviVovi.Application.Novels.Dtos;

public record NodeDto(
    Guid Id,
    string Name,
    ImageDto Image
);
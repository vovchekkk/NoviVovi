using NoviVovi.Application.Images.Dtos;
using NoviVovi.Application.Scene.Dtos;

namespace NoviVovi.Application.Characters.Dtos;

public record CharacterStateDto(
    Guid Id,
    string Name,
    string? Description,
    ImageDto Image,
    TransformDto LocalTransform
);
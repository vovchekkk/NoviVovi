using NoviVovi.Application.Images.Contracts;

namespace NoviVovi.Application.Characters.Contracts;

public record CharacterStateSnapshot(
    Guid Id,
    string Name,
    string? Description,
    ImageSnapshot Image
);
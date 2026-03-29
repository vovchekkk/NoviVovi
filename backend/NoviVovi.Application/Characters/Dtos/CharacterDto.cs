namespace NoviVovi.Application.Characters.Dtos;

public record CharacterDto
(
    Guid Id,
    string Name,
    string? Description,
    List<CharacterStateDto> CharacterStates
);
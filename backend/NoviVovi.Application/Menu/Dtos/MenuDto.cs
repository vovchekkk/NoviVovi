namespace NoviVovi.Application.Menu.Dtos;

public record MenuDto(
    Guid Id,
    string? Name,
    string? Description,
    string? Text,
    List<ChoiceDto> Choices
);
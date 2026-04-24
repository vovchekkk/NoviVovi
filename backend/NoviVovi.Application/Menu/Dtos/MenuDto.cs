namespace NoviVovi.Application.Menu.Dtos;

public record MenuDto(
    Guid Id,
    List<ChoiceDto> Choices
);
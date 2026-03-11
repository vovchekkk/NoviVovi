namespace NoviVovi.Application.Menu.Contracts;

public record MenuSnapshot(
    Guid Id,
    string? Name,
    string? Description,
    string? Text,
    List<ChoiceSnapshot> Choices
);
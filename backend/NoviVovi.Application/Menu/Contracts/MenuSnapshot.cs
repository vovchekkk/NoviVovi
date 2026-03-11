namespace NoviVovi.Application.Menu.Contracts;

public record MenuSnapshot(
    string? Name,
    string? Description,
    string? Text,
    List<ChoiceSnapshot> Choices
);
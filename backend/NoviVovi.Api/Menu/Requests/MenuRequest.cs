namespace NoviVovi.Api.Menu.Requests;

public record MenuRequest(
    Guid Id,
    string? Name,
    string? Description,
    string? Text,
    List<ChoiceRequest> Choices
);
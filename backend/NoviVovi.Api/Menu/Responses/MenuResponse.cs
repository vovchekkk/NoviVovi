namespace NoviVovi.Api.Menu.Responses;

public record MenuResponse(
    Guid Id,
    string? Name,
    string? Description,
    string? Text,
    List<ChoiceResponse> Choices
);
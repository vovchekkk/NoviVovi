namespace NoviVovi.Api.Menu.Responses;

public record MenuResponse(
    Guid Id,
    List<ChoiceResponse> Choices
);
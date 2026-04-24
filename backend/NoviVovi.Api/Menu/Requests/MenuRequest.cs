namespace NoviVovi.Api.Menu.Requests;

public record MenuRequest(
    Guid Id,
    List<ChoiceRequest> Choices
);
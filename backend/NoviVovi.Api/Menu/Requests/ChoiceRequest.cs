using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Menu.Requests;

public record ChoiceRequest(
    Guid Id,
    string? Name,
    string? Description,
    string? Text,
    ChoiceTransitionRequest Transition
);
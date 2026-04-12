using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Menu.Responses;

public record ChoiceResponse(
    Guid Id,
    string Name,
    string? Description,
    string Text,
    ChoiceTransitionResponse Transition
);
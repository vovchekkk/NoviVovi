using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Menu.Responses;

public record ChoiceResponse(
    Guid Id,
    string Text,
    ChoiceTransitionResponse Transition
);
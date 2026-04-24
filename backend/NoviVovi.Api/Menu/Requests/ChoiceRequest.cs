using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Menu.Requests;

public record ChoiceRequest(
    Guid Id,
    string Text,
    ChoiceTransitionRequest Transition
);
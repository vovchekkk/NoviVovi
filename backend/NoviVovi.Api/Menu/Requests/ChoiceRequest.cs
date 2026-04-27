using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Menu.Requests;

public record ChoiceRequest(
    string Text,
    ChoiceTransitionRequest Transition
);
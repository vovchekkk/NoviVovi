using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Menu.Responses;

public record ChoiceResponse(
    string Text,
    ChoiceTransitionResponse Transition
);
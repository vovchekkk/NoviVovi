using NoviVovi.Application.Transitions.Contracts;

namespace NoviVovi.Application.Menu.Contracts;

public record ChoiceSnapshot(
    Guid Id,
    string? Name,
    string? Description,
    string? Text,
    ChoiceTransitionSnapshot Transition
);
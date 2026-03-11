using NoviVovi.Domain.Steps.Transitions;

namespace NoviVovi.Application.Menu.Contracts;

public record ChoiceSnapshot(
    string? Name,
    string? Description,
    string? Text,
    ChoiceTransition Transition
);
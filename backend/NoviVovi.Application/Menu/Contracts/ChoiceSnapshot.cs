using NoviVovi.Application.Common.Contracts;
using NoviVovi.Domain.Steps.Transitions;

namespace NoviVovi.Application.Menu.Contracts;

public record ChoiceSnapshot(
    Guid Id,
    string? Name,
    string? Description,
    string? Text,
    ChoiceTransitionSnapshot Transition
);
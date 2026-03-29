using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Menu.Dtos;

public record ChoiceDto(
    Guid Id,
    string? Name,
    string? Description,
    string? Text,
    ChoiceTransitionDto Transition
);
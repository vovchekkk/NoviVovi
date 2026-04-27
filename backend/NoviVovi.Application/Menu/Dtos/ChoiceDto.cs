using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Menu.Dtos;

public record ChoiceDto(
    string Text,
    ChoiceTransitionDto Transition
);
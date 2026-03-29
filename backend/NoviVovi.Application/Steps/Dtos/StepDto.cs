using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record StepDto(
    Guid Id,
    TransitionDto Transition
);
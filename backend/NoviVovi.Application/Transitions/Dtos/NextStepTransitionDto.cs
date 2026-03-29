namespace NoviVovi.Application.Transitions.Dtos;

public record NextStepTransitionDto(
    Guid Id
) : TransitionDto(Id);
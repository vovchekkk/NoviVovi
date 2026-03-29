using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record JumpStepDto(
    Guid Id,
    TransitionDto Transition
) : StepDto(Id, Transition);
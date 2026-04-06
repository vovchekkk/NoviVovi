namespace NoviVovi.Application.Transitions.Dtos;

public record JumpTransitionDto(
    Guid Id,
    Guid TargetLabelId
) : TransitionDto(Id);
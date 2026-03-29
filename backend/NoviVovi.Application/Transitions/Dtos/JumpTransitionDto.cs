using NoviVovi.Application.Labels.Dtos;

namespace NoviVovi.Application.Transitions.Dtos;

public record JumpTransitionDto(
    Guid Id,
    LabelDto TargetLabel
) : TransitionDto(Id);
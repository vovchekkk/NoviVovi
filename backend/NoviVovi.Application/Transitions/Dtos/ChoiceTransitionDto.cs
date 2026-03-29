using NoviVovi.Application.Labels.Dtos;

namespace NoviVovi.Application.Transitions.Dtos;

public record ChoiceTransitionDto(
    Guid Id,
    LabelDto TargetLabel
) : TransitionDto(Id);
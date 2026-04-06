namespace NoviVovi.Application.Transitions.Dtos;

public record ChoiceTransitionDto(
    Guid Id,
    Guid TargetLabelId
) : TransitionDto(Id);
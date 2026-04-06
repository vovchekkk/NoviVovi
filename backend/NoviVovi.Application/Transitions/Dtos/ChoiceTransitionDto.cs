namespace NoviVovi.Application.Transitions.Dtos;

public record ChoiceTransitionDto : TransitionDto
{
    public required Guid TargetLabelId { get; init; }
}
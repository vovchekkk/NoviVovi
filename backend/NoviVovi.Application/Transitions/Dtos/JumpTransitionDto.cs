namespace NoviVovi.Application.Transitions.Dtos;

public record JumpTransitionDto : TransitionDto
{
    public required Guid TargetLabelId { get; init; }
}
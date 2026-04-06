namespace NoviVovi.Api.Transitions.Requests;

public record JumpTransitionRequest : TransitionRequest
{
    public required Guid TargetLabelId { get; init; }
}
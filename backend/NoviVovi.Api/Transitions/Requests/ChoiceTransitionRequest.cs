namespace NoviVovi.Api.Transitions.Requests;

public record ChoiceTransitionRequest : TransitionRequest
{
    public required Guid TargetLabelId { get; init; }
}
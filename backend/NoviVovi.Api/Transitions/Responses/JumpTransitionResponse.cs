using NoviVovi.Api.Transitions.Requests;

namespace NoviVovi.Api.Transitions.Responses;

public record JumpTransitionResponse : TransitionResponse
{
    public required Guid TargetLabelId { get; init; }
}
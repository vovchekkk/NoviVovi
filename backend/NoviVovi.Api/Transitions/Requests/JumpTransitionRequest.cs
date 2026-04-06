namespace NoviVovi.Api.Transitions.Requests;

public record JumpTransitionRequest(
    Guid Id,
    Guid TargetLabelId
) : TransitionRequest(Id);
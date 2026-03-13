namespace NoviVovi.Api.Transitions.Responses;

public record JumpTransitionResponse(
    Guid TargetLabelId
) : TransitionResponse;
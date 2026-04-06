using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Transitions.Responses;

public record JumpTransitionResponse(
    Guid Id,
    Guid TargetLabelId
) : TransitionResponse(Id);
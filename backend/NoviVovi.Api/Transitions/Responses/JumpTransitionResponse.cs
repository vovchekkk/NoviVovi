using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Transitions.Responses;

public record JumpTransitionResponse(
    Guid Id,
    LabelResponse TargetLabel
) : TransitionResponse(Id);
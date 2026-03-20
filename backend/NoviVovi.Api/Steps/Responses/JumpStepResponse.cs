using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record JumpStepResponse(
    Guid Id,
    LabelResponse TargetLabel,
    TransitionResponse Transition
) : StepResponse(Id, Transition);
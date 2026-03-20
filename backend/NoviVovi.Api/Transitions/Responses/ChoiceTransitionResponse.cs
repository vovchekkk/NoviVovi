using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Transitions.Responses;

public record ChoiceTransitionResponse(
    Guid Id,
    LabelResponse TargetLabel
) : TransitionResponse(Id);
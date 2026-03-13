namespace NoviVovi.Api.Transitions.Responses;

public record ChoiceTransitionResponse(
    Guid TargetLabelId
) : TransitionResponse;
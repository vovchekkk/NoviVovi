namespace NoviVovi.Api.Transitions.Responses;

public record ChoiceTransitionResponse(
    Guid Id,
    Guid TargetLabelId
) : TransitionResponse(Id);
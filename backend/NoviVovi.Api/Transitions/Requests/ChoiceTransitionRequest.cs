namespace NoviVovi.Api.Transitions.Requests;

public record ChoiceTransitionRequest(
    Guid Id,
    Guid TargetLabelId
) : TransitionRequest(Id);
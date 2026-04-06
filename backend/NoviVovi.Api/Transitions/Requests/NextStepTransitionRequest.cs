namespace NoviVovi.Api.Transitions.Requests;

public record NextStepTransitionRequest(
    Guid Id
) : TransitionRequest(Id);
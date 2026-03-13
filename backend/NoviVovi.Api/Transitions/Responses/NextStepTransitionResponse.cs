namespace NoviVovi.Api.Transitions.Responses;

public record NextStepTransitionResponse(
    Guid Id
) : TransitionResponse(Id);
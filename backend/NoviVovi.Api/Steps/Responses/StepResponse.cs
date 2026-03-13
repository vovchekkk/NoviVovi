using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public abstract record StepResponse(
    Guid Id,
    TransitionResponse Transition
);
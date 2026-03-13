using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public record StepResponse(
    Guid Id,
    TransitionResponse Transition
);
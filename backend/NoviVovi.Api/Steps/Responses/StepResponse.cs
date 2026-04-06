using NoviVovi.Api.Transitions.Responses;

namespace NoviVovi.Api.Steps.Responses;

public abstract record StepResponse
{
    public required Guid Id { get; init; }
}

public abstract record StepResponse<TTransition> : StepResponse 
    where TTransition : TransitionResponse
{
    public required TTransition Transition { get; init; }
}
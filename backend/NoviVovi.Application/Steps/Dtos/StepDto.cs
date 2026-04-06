using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public abstract record StepDto
{
    public required Guid Id { get; init; }
}

public abstract record StepDto<TTransition> : StepDto 
    where TTransition : TransitionDto
{
    public required TTransition Transition { get; init; }
}
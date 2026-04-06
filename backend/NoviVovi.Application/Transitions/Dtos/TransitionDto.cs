namespace NoviVovi.Application.Transitions.Dtos;

public abstract record TransitionDto
{
    public required Guid Id { get; init; }
}
namespace NoviVovi.Api.Transitions.Responses;

public abstract record TransitionResponse
{
    public required Guid Id { get; init; }
}
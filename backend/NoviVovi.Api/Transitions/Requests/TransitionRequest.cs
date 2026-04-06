namespace NoviVovi.Api.Transitions.Requests;

public abstract record TransitionRequest
{
    public required Guid Id { get; init; }
}
namespace NoviVovi.Api.Novels.Responses;

public record NodeResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
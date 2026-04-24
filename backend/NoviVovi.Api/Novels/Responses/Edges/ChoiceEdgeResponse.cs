namespace NoviVovi.Api.Novels.Responses.Edges;

public record ChoiceEdgeResponse : EdgeResponse
{
    public required Guid SourceChoiceId { get; init; }
}
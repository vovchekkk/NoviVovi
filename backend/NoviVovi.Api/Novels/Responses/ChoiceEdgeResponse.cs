using NoviVovi.Api.Menu.Responses;

namespace NoviVovi.Api.Novels.Responses;

public record ChoiceEdgeResponse : EdgeResponse
{
    public required Guid ChoiceId { get; init; }
    public required string Name { get; init; }
}
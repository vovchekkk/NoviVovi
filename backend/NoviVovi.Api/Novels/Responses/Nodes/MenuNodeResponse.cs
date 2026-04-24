using NoviVovi.Api.Menu.Responses;

namespace NoviVovi.Api.Novels.Responses.Nodes;

public record MenuNodeResponse : NodeResponse
{
    public required IReadOnlyCollection<ChoiceResponse> Choices { get; init; }
}
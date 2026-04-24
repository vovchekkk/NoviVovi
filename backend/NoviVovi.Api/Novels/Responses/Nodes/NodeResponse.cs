using System.Text.Json.Serialization;

namespace NoviVovi.Api.Novels.Responses.Nodes;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(JumpNodeResponse), "jump")]
[JsonDerivedType(typeof(MenuNodeResponse), "choice")]
public record NodeResponse
{
    public required Guid LabelId { get; init; }
    public required string LabelName { get; init; }
}
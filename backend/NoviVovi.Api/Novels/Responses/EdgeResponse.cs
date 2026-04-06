using System.Text.Json.Serialization;
using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Novels.Responses;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(JumpEdgeResponse), "jump")]
[JsonDerivedType(typeof(ChoiceEdgeResponse), "choice")]
public abstract record EdgeResponse
{
    public required string Id { get; init; }
    public required Guid SourceLabelId { get; init; }
    public required Guid TargetLabelId { get; init; }
}
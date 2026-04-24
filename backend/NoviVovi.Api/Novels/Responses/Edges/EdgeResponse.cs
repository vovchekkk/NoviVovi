using System.Text.Json.Serialization;

namespace NoviVovi.Api.Novels.Responses.Edges;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(JumpEdgeResponse), "jump")]
[JsonDerivedType(typeof(ChoiceEdgeResponse), "choice")]
public abstract record EdgeResponse
{
    public required Guid StepId { get; init; }
    public required Guid SourceLabelId { get; init; }
    public required Guid TargetLabelId { get; init; }
}
using System.Text.Json.Serialization;
using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Novels.Responses;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(JumpEdgeResponse), "jump")]
[JsonDerivedType(typeof(ChoiceEdgeResponse), "choice")]
public record EdgeResponse(
    Guid Id,
    Guid SourceLabelId,
    Guid TargetLabelID
);
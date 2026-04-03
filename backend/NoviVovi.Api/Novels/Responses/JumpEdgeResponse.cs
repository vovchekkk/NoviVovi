using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Novels.Responses;

public record JumpEdgeResponse(
    Guid Id,
    LabelResponse SourceLabel,
    LabelResponse TargetLabel
) : EdgeResponse(Id, SourceLabel, TargetLabel);
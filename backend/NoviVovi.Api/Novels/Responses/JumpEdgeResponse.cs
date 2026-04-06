using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Novels.Responses;

public record JumpEdgeResponse(
    Guid Id,
    Guid SourceLabelId,
    Guid TargetLabelId
) : EdgeResponse(Id, SourceLabelId, TargetLabelId);
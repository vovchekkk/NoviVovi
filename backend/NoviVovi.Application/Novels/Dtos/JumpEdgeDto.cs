using NoviVovi.Application.Labels.Dtos;

namespace NoviVovi.Application.Novels.Dtos;

public record JumpEdgeDto(
    Guid Id,
    Guid SourceLabelId,
    Guid TargetLabelId
) : EdgeDto(Id, SourceLabelId, TargetLabelId);
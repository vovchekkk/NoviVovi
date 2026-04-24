namespace NoviVovi.Application.Novels.Dtos.Edges;

public record JumpEdgeDto(
    Guid StepId,
    Guid SourceLabelId,
    Guid TargetLabelId
) : EdgeDto(StepId, SourceLabelId, TargetLabelId);
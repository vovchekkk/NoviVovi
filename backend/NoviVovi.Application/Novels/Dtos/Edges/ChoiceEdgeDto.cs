namespace NoviVovi.Application.Novels.Dtos.Edges;

public record ChoiceEdgeDto(
    Guid StepId,
    Guid SourceLabelId,
    Guid TargetLabelId,
    Guid SourceChoiceId
) : EdgeDto(StepId, SourceLabelId, TargetLabelId);
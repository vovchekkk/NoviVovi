namespace NoviVovi.Application.Novels.Dtos.Edges;

public abstract record EdgeDto(
    Guid StepId,
    Guid SourceLabelId,
    Guid TargetLabelId
);
using NoviVovi.Application.Labels.Dtos;

namespace NoviVovi.Application.Novels.Dtos;

public record JumpEdgeDto(
    Guid Id,
    LabelDto SourceLabel,
    LabelDto TargetLabel
) : EdgeDto(Id, SourceLabel, TargetLabel);
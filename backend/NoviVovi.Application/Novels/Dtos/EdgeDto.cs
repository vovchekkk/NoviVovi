using NoviVovi.Application.Labels.Dtos;

namespace NoviVovi.Application.Novels.Dtos;

public record EdgeDto(
    Guid Id,
    LabelDto SourceLabel,
    LabelDto TargetLabel
);
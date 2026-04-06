using NoviVovi.Application.Labels.Dtos;

namespace NoviVovi.Application.Novels.Dtos;

public record EdgeDto(
    Guid Id,
    Guid SourceLabelId,
    Guid TargetLabelId
);
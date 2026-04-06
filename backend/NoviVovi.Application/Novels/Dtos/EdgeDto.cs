using NoviVovi.Application.Labels.Dtos;

namespace NoviVovi.Application.Novels.Dtos;

public abstract record EdgeDto(
    Guid Id,
    Guid SourceLabelId,
    Guid TargetLabelId
);
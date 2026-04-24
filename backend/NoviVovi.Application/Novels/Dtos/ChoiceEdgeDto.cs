using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Menu.Dtos;

namespace NoviVovi.Application.Novels.Dtos;

public record ChoiceEdgeDto(
    Guid Id,
    Guid SourceLabelId,
    Guid TargetLabelId,
    ChoiceDto Choice,
    string Text
) : EdgeDto(Id, SourceLabelId, TargetLabelId);
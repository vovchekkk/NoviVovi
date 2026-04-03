using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Menu.Dtos;

namespace NoviVovi.Application.Novels.Dtos;

public record ChoiceEdgeDto(
    Guid Id,
    LabelDto SourceLabel,
    LabelDto TargetLabel,
    ChoiceDto Choice,
    string Text
) : EdgeDto(Id, SourceLabel, TargetLabel);
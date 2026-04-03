using NoviVovi.Api.Labels.Responses;
using NoviVovi.Api.Menu.Responses;

namespace NoviVovi.Api.Novels.Responses;

public record ChoiceEdgeResponse(
    Guid Id,
    LabelResponse SourceLabel,
    LabelResponse TargetLabel,
    ChoiceResponse Choice,
    string Text
) : EdgeResponse(Id, SourceLabel, TargetLabel);
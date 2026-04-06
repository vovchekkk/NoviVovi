using NoviVovi.Api.Menu.Responses;

namespace NoviVovi.Api.Novels.Responses;

public record ChoiceEdgeResponse(
    Guid Id,
    Guid SourceLabelId,
    Guid TargetLabelId,
    ChoiceResponse Choice,
    string Text
) : EdgeResponse(Id, SourceLabelId, TargetLabelId);
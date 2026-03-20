using NoviVovi.Api.Labels.Responses;

namespace NoviVovi.Api.Novels.Responses;

public record NovelResponse(
    string Title,
    LabelResponse StartLabel,
    List<LabelResponse> Labels
);
namespace NoviVovi.Api.Novels.Responses;

public record NovelResponse(
    string Title,
    Guid StartLabelId,
    List<Guid> LabelIds
);
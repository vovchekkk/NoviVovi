namespace NoviVovi.Application.Novels.Contracts;

public record NovelSnapshot(
    string Title,
    Guid StartLabelId,
    List<Guid> LabelIds
);
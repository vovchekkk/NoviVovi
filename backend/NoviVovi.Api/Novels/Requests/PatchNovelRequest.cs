namespace NoviVovi.Api.Novels.Requests;

public record PatchNovelRequest(
    string? Title = null,
    Guid? StartLabelId = null
);
namespace NoviVovi.Api.Novels.Requests.Patch;

public record PatchNovelRequest(
    Guid NovelId,
    string? Title = null,
    Guid? StartLabelId = null
);
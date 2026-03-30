namespace NoviVovi.Api.Novels.Requests.Patch;

public record PatchNovelRequest(
    string? Title = null,
    Guid? StartLabelId = null
);
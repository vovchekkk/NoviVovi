namespace NoviVovi.Api.Novels.Requests.Create;

public record CreateNovelRequest(
    string Title,
    Guid StartLabelId
);
namespace NoviVovi.Api.Labels.Requests.Patch;

public record PatchLabelRequest(
    Guid NovelId,
    Guid LabelId,
    string? Name = null
);
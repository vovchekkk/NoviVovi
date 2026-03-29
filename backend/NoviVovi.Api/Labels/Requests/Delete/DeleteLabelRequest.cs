namespace NoviVovi.Api.Labels.Requests.Delete;

public record DeleteLabelRequest(
    Guid NovelId,
    Guid LabelId
);
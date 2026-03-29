namespace NoviVovi.Api.Labels.Requests.Get;

public record GetLabelRequest(
    Guid NovelId,
    Guid LabelId
);
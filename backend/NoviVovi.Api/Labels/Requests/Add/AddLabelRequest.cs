namespace NoviVovi.Api.Labels.Requests.Add;

public record AddLabelRequest(
    Guid NovelId,
    string Name
);
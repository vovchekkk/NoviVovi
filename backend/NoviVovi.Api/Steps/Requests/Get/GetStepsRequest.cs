namespace NoviVovi.Api.Steps.Requests.Get;

public record GetStepsRequest(
    Guid NovelId,
    Guid LabelId
);
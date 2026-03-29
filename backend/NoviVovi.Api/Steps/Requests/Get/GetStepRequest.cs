namespace NoviVovi.Api.Steps.Requests.Get;

public record GetStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid StepId
);
namespace NoviVovi.Api.Steps.Requests.Delete;

public record DeleteStepRequest(
    Guid NovelId,
    Guid LabelId,
    Guid StepId
);
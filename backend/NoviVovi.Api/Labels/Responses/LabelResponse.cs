using NoviVovi.Api.Steps.Responses;

namespace NoviVovi.Api.Labels.Responses;

public record LabelResponse(
    Guid Id,
    string Name,
    Guid NovelId,
    List<StepResponse> Steps
);
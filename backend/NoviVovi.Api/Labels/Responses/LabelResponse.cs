using NoviVovi.Api.Steps.Responses;

namespace NoviVovi.Api.Labels.Responses;

public record LabelResponse(
    Guid Id,
    Guid NovelId,
    string Name,
    List<StepResponse> Steps
);
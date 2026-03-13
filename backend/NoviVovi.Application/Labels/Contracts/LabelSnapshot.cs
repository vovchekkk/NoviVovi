using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Labels.Contracts;

public record LabelSnapshot(
    Guid Id,
    Guid NovelId,
    string Name,
    List<StepSnapshot> Steps
);
using NoviVovi.Application.Steps.Contracts;

namespace NoviVovi.Application.Labels.Contracts;

public record LabelSnapshot(
    Guid Id,
    string Name,
    List<StepSnapshot> Steps
);
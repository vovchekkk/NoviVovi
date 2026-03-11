using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Labels.Contracts;

public record LabelSnapshot(
    Guid Id,
    string Name,
    List<Step> Steps
);
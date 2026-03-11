using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Labels.Contracts;

public record LabelSnapshot(
    string Name,
    List<Step> Steps
);
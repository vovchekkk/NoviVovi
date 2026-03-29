using NoviVovi.Application.Steps.Dtos;

namespace NoviVovi.Application.Labels.Dtos;

public record LabelDto(
    Guid Id,
    string Name,
    List<StepDto> Steps
);
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record ShowBackgroundStepDto : StepDto<NextStepTransitionDto>
{
    public required BackgroundObjectDto BackgroundObject { get; init; }
}
using NoviVovi.Application.Menu.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record ShowMenuStepDto : StepDto<NextStepTransitionDto>
{
    public required MenuDto Menu { get; init; }
}
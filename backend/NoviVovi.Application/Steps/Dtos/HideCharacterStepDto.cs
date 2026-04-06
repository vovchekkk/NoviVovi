using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record HideCharacterStepDto : StepDto<NextStepTransitionDto>
{
    public required Guid CharacterId { get; init; }
}
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record ShowCharacterStepDto : StepDto<NextStepTransitionDto>
{
    public required CharacterObjectDto CharacterObject { get; init; }
}
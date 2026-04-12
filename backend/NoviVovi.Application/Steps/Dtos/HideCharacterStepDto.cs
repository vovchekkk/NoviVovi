using NoviVovi.Application.Characters.Dtos;
using NoviVovi.Application.Scene.Dtos;
using NoviVovi.Application.Transitions.Dtos;

namespace NoviVovi.Application.Steps.Dtos;

public record HideCharacterStepDto : StepDto<NextStepTransitionDto>
{
    public required CharacterDto Character { get; init; }
}
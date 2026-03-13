using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Application.Transitions.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Mappers;

public partial class HideCharacterStepMapper(
    CharacterMapper characterMapper,
    TransitionMapper transitionMapper
)
{
    public HideCharacterStepSnapshot ToSnapshot(HideCharacterStep step)
    {
        return new HideCharacterStepSnapshot(
            step.Id,
            characterMapper.ToSnapshot(step.Character),
            transitionMapper.ToSnapshot(step.Transition)
        );
    }
}
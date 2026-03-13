using NoviVovi.Application.Characters.Mappers;
using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Application.Transitions.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Mappers;

public partial class ShowCharacterStepMapper(
    CharacterMapper characterMapper,
    CharacterStateMapper characterStateMapper,
    TransformMapper transformMapper,
    TransitionMapper transitionMapper)
{
    public ShowCharacterStepSnapshot ToSnapshot(ShowCharacterStep step)
    {
        return new ShowCharacterStepSnapshot(
            step.Id,
            characterMapper.ToSnapshot(step.Character),
            characterStateMapper.ToSnapshot(step.State),
            transformMapper.ToSnapshot(step.Transform),
            transitionMapper.ToSnapshot(step.Transition)
        );
    }
}
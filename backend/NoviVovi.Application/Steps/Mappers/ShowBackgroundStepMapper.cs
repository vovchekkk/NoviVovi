using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Images.Mappers;
using NoviVovi.Application.Scene.Mappers;
using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Application.Transitions.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Mappers;

public partial class ShowBackgroundStepMapper(
    ImageMapper imageMapper,
    TransformMapper transformMapper,
    TransitionMapper transitionMapper)
{
    public ShowBackgroundStepSnapshot ToSnapshot(ShowBackgroundStep step)
    {
        return new ShowBackgroundStepSnapshot(
            step.Id,
            imageMapper.ToSnapshot(step.Background),
            transformMapper.ToSnapshot(step.Transform),
            transitionMapper.ToSnapshot(step.Transition)
        );
    }
}
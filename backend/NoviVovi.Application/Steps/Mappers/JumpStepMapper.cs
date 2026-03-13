using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Application.Transitions.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Mappers;

public partial class JumpStepMapper(
    LabelMapper labelMapper,
    TransitionMapper transitionMapper
)
{
    public JumpStepSnapshot ToSnapshot(JumpStep step)
    {
        return new JumpStepSnapshot(
            step.Id,
            labelMapper.ToSnapshot(step.Label),
            transitionMapper.ToSnapshot(step.Transition)
        );
    }
}
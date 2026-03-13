using NoviVovi.Application.Dialogue.Mappers;
using NoviVovi.Application.Menu.Mappers;
using NoviVovi.Application.Steps.Contracts;
using NoviVovi.Application.Transitions.Mappers;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Mappers;

public partial class ShowMenuStepMapper(MenuMapper menuMapper, TransitionMapper transitionMapper)
{
    public ShowMenuStepSnapshot ToSnapshot(ShowMenuStep step)
    {
        return new ShowMenuStepSnapshot(
            step.Id,
            menuMapper.ToSnapshot(step.Menu),
            transitionMapper.ToSnapshot(step.Transition)
        );
    }
}
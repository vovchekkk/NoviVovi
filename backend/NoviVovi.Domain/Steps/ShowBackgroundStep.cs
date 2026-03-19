using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowBackgroundStep : Step
{
    public BackgroundObject BackgroundObject { get; }

    private ShowBackgroundStep(
        Guid id,
        BackgroundObject backgroundObject,
        Transition transition
    ) : base(id, transition)
    {
        BackgroundObject = backgroundObject;
    }

    public static ShowBackgroundStep Create(
        BackgroundObject backgroundObject
    )
    {
        return new ShowBackgroundStep(Guid.NewGuid(), backgroundObject, NextStepTransition.Create());
    }

    public static ShowBackgroundStep Rehydrate(
        Guid id,
        BackgroundObject backgroundObject,
        Transition transition
    )
    {
        return new ShowBackgroundStep(id, backgroundObject, transition);
    }
}
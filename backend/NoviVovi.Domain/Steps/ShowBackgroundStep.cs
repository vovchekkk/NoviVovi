using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowBackgroundStep : Step
{
    public BackgroundObject BackgroundObject { get; }

    private ShowBackgroundStep(
        Guid id,
        BackgroundObject backgroundObject,
        NextStepTransition transition
    ) : base(id, transition)
    {
        BackgroundObject = backgroundObject;
    }

    public static ShowBackgroundStep Create(
        BackgroundObject? backgroundObject
    )
    {
        if (backgroundObject is null)
            throw new DomainException($"BackgroundObject cannot be null");
        
        return new ShowBackgroundStep(Guid.NewGuid(), backgroundObject, NextStepTransition.Create());
    }
    
    public new NextStepTransition Transition => (NextStepTransition)base.Transition;
}
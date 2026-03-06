using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps.Transitions;

namespace NoviVovi.Domain.Steps;

public abstract class Step : Entity
{
    public StepTransition Transition { get; }

    protected Step(Guid id, StepTransition transition) : base(id)
    {
        Transition = transition;
    }
}
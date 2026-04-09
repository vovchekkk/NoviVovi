using NoviVovi.Domain.Common;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public abstract class Step(Guid id, Transition transition) : Entity(id)
{
    public Transition Transition { get; private set; } = transition;
}
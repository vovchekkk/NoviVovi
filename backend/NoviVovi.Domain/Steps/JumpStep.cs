using NoviVovi.Domain.Common;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class JumpStep : Step
{
    private JumpStep(Guid id, Transition transition) : base(id, transition)
    {
    }

    public static JumpStep Create(Guid targetLabelId)
    {
        if (targetLabelId == Guid.Empty)
            throw new DomainException($"TargetLabelId {targetLabelId} cannot be empty");

        return new JumpStep(Guid.NewGuid(), JumpTransition.Create(targetLabelId));
    }

    public static JumpStep Rehydrate(Guid id, Transition transition)
    {
        return new JumpStep(id, transition);
    }
}
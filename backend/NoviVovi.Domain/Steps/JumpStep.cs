using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class JumpStep : Step
{
    private JumpStep(Guid id, JumpTransition transition) : base(id, transition)
    {
    }

    public static JumpStep Create(Label? targetLabel)
    {
        if (targetLabel is null)
            throw new DomainException($"TargetLabel cannot be null");

        return new JumpStep(Guid.NewGuid(), JumpTransition.Create(targetLabel));
    }
    
    public new JumpTransition Transition => (JumpTransition)base.Transition;
}
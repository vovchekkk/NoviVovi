using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Domain.Transitions;

public sealed class JumpTransition : Transition
{
    public Label TargetLabel { get; private set; }

    public JumpTransition(Label targetLabel)
    {
        TargetLabel = targetLabel;
    }

    public static JumpTransition Create(Label? targetLabel)
    {
        if (targetLabel is null)
            throw new DomainException($"TargetLabel cannot be null");

        return new JumpTransition(targetLabel);
    }
    
    public void ApplyTargetLabel(Label? targetLabel)
    {
        TargetLabel = targetLabel
                      ?? throw new DomainException($"TargetLabel cannot be null");
    }
}
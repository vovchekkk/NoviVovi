using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Domain.Transitions;

public sealed class ChoiceTransition : Transition
{
    public Label TargetLabel { get; private set; }
    
    private ChoiceTransition(Label targetLabel)
    {
        TargetLabel = targetLabel;
    }

    public static ChoiceTransition Create(Label? targetLabel)
    {
        if (targetLabel is null)
            throw new DomainException($"TargetLabel cannot be null");

        return new ChoiceTransition(targetLabel);
    }
}
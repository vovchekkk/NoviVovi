using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Domain.Transitions;

public sealed class ChoiceTransition : Transition
{
    public Label TargetLabel { get; private set; }

    public ChoiceTransition(Label targetLabel)
    {
        TargetLabel = targetLabel;
    }

    public static ChoiceTransition Create(Label? targetLabel)
    {
        if (targetLabel is null)
            throw new DomainException($"TargetLabel cannot be null");

        return new ChoiceTransition(targetLabel);
    }
    
    public void ApplyTargetLabel(Label? targetLabel)
    {
        TargetLabel = targetLabel
                      ?? throw new DomainException($"TargetLabel cannot be null");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TargetLabel;
    }
}
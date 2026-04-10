using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Domain.Transitions;

public sealed class ChoiceTransition : Transition
{
    public Label TargetLabel { get; }

    public ChoiceTransition(Guid id, Label targetLabel) : base(id)
    {
        TargetLabel = targetLabel;
    }

    public static ChoiceTransition Create(Label? targetLabel)
    {
        if (targetLabel is null)
            throw new DomainException($"TargetLabel cannot be null");

        return new ChoiceTransition(Guid.NewGuid(), targetLabel);
    }
}
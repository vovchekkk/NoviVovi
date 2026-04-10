using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Domain.Transitions;

public sealed class JumpTransition : Transition
{
    public Label TargetLabel { get; }

    public JumpTransition(Guid id, Label targetLabel) : base(id)
    {
        TargetLabel = targetLabel;
    }

    public static JumpTransition Create(Label? targetLabel)
    {
        if (targetLabel is null)
            throw new DomainException($"TargetLabel cannot be null");

        return new JumpTransition(Guid.NewGuid(), targetLabel);
    }
}
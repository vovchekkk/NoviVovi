using NoviVovi.Domain.Labels;

namespace NoviVovi.Domain.Steps.Transitions;

public sealed class JumpTransition : StepTransition
{
    public Label TargetLabel { get; }

    public JumpTransition(Label targetLabel)
    {
        TargetLabel = targetLabel ?? throw new ArgumentNullException(nameof(targetLabel));
    }
}
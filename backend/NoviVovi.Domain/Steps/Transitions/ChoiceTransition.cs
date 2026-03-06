using NoviVovi.Domain.Labels;

namespace NoviVovi.Domain.Steps.Transitions;

public sealed class ChoiceTransition : StepTransition
{
    public Label TargetLabel { get; }

    public ChoiceTransition(Label targetLabel)
    {
        TargetLabel = targetLabel ?? throw new ArgumentNullException(nameof(targetLabel));
    }
}
using NoviVovi.Domain.Labels;

namespace NoviVovi.Domain.Steps.Transitions;

public sealed class JumpTransition(Guid targetLabelId) : StepTransition
{
    public Guid TargetLabelId { get; } = targetLabelId == Guid.Empty
        ? throw new ArgumentNullException(nameof(targetLabelId))
        : targetLabelId;
}
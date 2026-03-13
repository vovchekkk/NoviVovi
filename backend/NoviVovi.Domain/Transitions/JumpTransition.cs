namespace NoviVovi.Domain.Transitions;

public sealed class JumpTransition(Guid id, Guid targetLabelId) : Transition(id)
{
    public Guid TargetLabelId { get; } = targetLabelId == Guid.Empty
        ? throw new ArgumentNullException(nameof(targetLabelId))
        : targetLabelId;
}
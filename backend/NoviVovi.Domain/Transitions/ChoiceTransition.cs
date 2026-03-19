using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Transitions;

public sealed class ChoiceTransition : Transition
{
    public Guid TargetLabelId { get; }
    
    private ChoiceTransition(Guid id, Guid targetLabelId) : base(id)
    {
        TargetLabelId = targetLabelId;
    }

    public static ChoiceTransition Create(Guid targetLabelId)
    {
        if (targetLabelId == Guid.Empty)
            throw new DomainException($"TargetLabelId {targetLabelId} cannot be empty");

        return new ChoiceTransition(Guid.NewGuid(), targetLabelId);
    }

    public static ChoiceTransition Rehydrate(Guid id, Guid targetLabelId)
    {
        return new ChoiceTransition(id, targetLabelId);
    }
}
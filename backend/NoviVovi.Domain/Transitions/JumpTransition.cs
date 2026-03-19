using NoviVovi.Domain.Common;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Domain.Transitions;

public sealed class JumpTransition : Transition
{
    public Guid TargetLabelId { get; }
    
    private JumpTransition(Guid id, Guid targetLabelId) : base(id)
    {
        TargetLabelId = targetLabelId;
    }

    public static JumpTransition Create(Guid targetLabelId)
    {
        if (targetLabelId == Guid.Empty)
            throw new DomainException($"TargetLabelId {targetLabelId} cannot be empty");

        return new JumpTransition(Guid.NewGuid(), targetLabelId);
    }

    public static JumpTransition Rehydrate(Guid id, Guid targetLabelId)
    {
        return new JumpTransition(id, targetLabelId);
    }
}
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowReplicaStep : Step
{
    public Replica Replica { get; }

    public ShowReplicaStep(Guid id, Replica replica, NextStepTransition transition) : base(id, transition)
    {
        Replica = replica;
    }

    public static ShowReplicaStep Create(Replica? replica)
    {
        if (replica is null)
            throw new DomainException($"Replica cannot be null");
        
        return new ShowReplicaStep(Guid.NewGuid(), replica, NextStepTransition.Create());
    }
    
    public new NextStepTransition Transition => (NextStepTransition)base.Transition;
}
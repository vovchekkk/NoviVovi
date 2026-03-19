using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowReplicaStep : Step
{
    public Replica Replica { get; }
    
    private ShowReplicaStep(Guid id, Replica replica, Transition transition) : base(id, transition)
    {
        Replica = replica;
    }

    public static ShowReplicaStep Create(Replica replica)
    {
        return new ShowReplicaStep(Guid.NewGuid(), replica, NextStepTransition.Create());
    }

    public static ShowReplicaStep Rehydrate(Guid id, Replica replica, Transition transition)
    {
        return new ShowReplicaStep(id, replica, transition);
    }
}
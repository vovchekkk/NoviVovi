using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowReplicaStep(Guid id, Replica replica, NextStepTransition transition) : Step(id, transition)
{
    public Replica Replica { get; } = replica;
}
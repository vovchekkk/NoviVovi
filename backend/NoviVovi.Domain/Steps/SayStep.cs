using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps.Transitions;

namespace NoviVovi.Domain.Steps;

public class SayStep(Guid id, Replica replica, NextStepTransition transition) : Step(id, transition)
{
    public Replica Replica { get; } = replica;
}
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class HideCharacterStep(Guid id, Character character, NextStepTransition transition) : Step(id, transition)
{
    public Character Character { get; } = character;
}
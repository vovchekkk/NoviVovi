using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class HideCharacterStep : Step
{
    public Character Character { get; }
    
    private HideCharacterStep(Guid id, Character character, Transition transition) : base(id, transition)
    {
        Character = character;
    }

    public static HideCharacterStep Create(Character character)
    {
        return new HideCharacterStep(Guid.NewGuid(), character, NextStepTransition.Create());
    }

    public static HideCharacterStep Rehydrate(Guid id, Character character, Transition transition)
    {
        return new HideCharacterStep(id, character, transition);
    }
}
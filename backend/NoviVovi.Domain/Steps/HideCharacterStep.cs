using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class HideCharacterStep : Step
{
    public Character Character { get; private set; }

    public HideCharacterStep(
        Guid id,
        Character character,
        NextStepTransition transition
    ) : base(id, transition)
    {
        Character = character;
    }

    public static HideCharacterStep Create(Character? character)
    {
        if (character is null)
            throw new DomainException($"Character cannot be null");

        return new HideCharacterStep(Guid.NewGuid(), character, NextStepTransition.Create());
    }

    public void Update(Character? character)
    {
        if (character is not null)
            Character = character;
    }

    public new NextStepTransition Transition => (NextStepTransition)base.Transition;
}
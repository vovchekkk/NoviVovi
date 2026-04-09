using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class HideCharacterStep : Step
{
    public CharacterObject CharacterObject { get; private set; }

    private HideCharacterStep(
        Guid id,
        CharacterObject characterObject,
        NextStepTransition transition
    ) : base(id, transition)
    {
        CharacterObject = characterObject;
    }

    public static HideCharacterStep Create(CharacterObject? characterObject)
    {
        if (characterObject is null)
            throw new DomainException($"CharacterObject cannot be null");

        return new HideCharacterStep(Guid.NewGuid(), characterObject, NextStepTransition.Create());
    }

    public new NextStepTransition Transition => (NextStepTransition)base.Transition;
}
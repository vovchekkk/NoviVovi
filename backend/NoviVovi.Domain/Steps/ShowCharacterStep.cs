using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowCharacterStep : Step
{
    public CharacterObject CharacterObject { get; }

    private ShowCharacterStep(
        Guid id,
        CharacterObject characterObject,
        Transition transition
    ) : base(id, transition)
    {
        CharacterObject = characterObject;
    }

    public static ShowCharacterStep Create(
        CharacterObject? characterObject
    )
    {
        if (characterObject is null)
            throw new DomainException($"CharacterObject cannot be null");
        
        return new ShowCharacterStep(Guid.NewGuid(), characterObject, NextStepTransition.Create());
    }
}
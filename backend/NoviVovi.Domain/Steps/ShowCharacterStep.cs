using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowCharacterStep : Step
{
    public CharacterObject CharacterObject { get; private set; }

    public ShowCharacterStep(
        Guid id,
        CharacterObject characterObject,
        NextStepTransition transition
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

    public void Update(Character? character, CharacterState? state, TransformPatch? transformPatch)
    {
        if (character is not null) 
            CharacterObject.UpdateCharacter(character);
        
        if (state is not null) 
            CharacterObject.UpdateCharacterState(state);
        
        if (transformPatch is not null)
            CharacterObject.PatchTransform(transformPatch);
    }
    
    public new NextStepTransition Transition => (NextStepTransition)base.Transition;
}
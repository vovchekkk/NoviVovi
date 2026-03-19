using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowCharacterStep : Step
{
    public Character Character { get; }
    public CharacterState State { get; }
    public Transform Transform { get; }

    private ShowCharacterStep(
        Guid id,
        Character character,
        CharacterState state,
        Transform transform,
        Transition transition
    ) : base(id, transition)
    {
        Character = character;
        State = state;
        Transform = transform;
    }

    public static ShowCharacterStep Create(
        Character character,
        CharacterState state,
        Transform transform
    )
    {
        return new ShowCharacterStep(Guid.NewGuid(), character, state, transform, NextStepTransition.Create());
    }

    public static ShowCharacterStep Rehydrate(
        Guid id,
        Character character,
        CharacterState state,
        Transform transform,
        Transition transition
    )
    {
        return new ShowCharacterStep(id, character, state, transform, transition);
    }
}
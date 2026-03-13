using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowCharacterStep(
    Guid id,
    Character character,
    CharacterState state,
    Transform transform,
    NextStepTransition transition)
    : Step(id, transition)
{
    public Character Character { get; } = character;
    public CharacterState State { get; } = state;
    public Transform Transform { get; } = transform;
}
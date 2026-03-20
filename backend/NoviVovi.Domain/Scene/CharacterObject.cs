using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;

namespace NoviVovi.Domain.Scene;

public class CharacterObject : SceneObject
{
    public Character Character { get; }
    public CharacterState State { get; private set; }

    private CharacterObject(
        Guid id,
        Character character,
        CharacterState state,
        Transform transform)
        : base(id, transform)
    {
        Character = character;
        State = state;
    }

    public static CharacterObject Create(
        Character? character,
        CharacterState? state,
        Transform? transform)
    {
        if (character is null)
            throw new DomainException($"Character cannot be null");

        if (state is null)
            throw new DomainException($"CharacterState cannot be null");

        if (transform is null)
            throw new DomainException($"Transform cannot be null");

        return new CharacterObject(Guid.NewGuid(), character, state, transform);
    }
}
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Characters;

public class Character : Entity
{
    public string Name { get; private set; }
    public string NameColor { get; private set; }
    public string? Description { get; private set; }
    private readonly List<CharacterState> _characterStates = new();

    public IReadOnlyList<CharacterState> CharacterStates => _characterStates.AsReadOnly();

    public Character(Guid id, string name, string nameColor, string? description) : base(id)
    {
        Name = name;
        NameColor = nameColor;
        Description = description;
    }

    public static Character Create(string? name, string? nameColor, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");
        
        if (string.IsNullOrWhiteSpace(nameColor))
            throw new DomainException($"NameColor cannot be empty");

        return new Character(Guid.NewGuid(), name, nameColor, description);
    }

    public void UpdateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");

        Name = name;
    }

    public void UpdateNameColor(string? nameColor)
    {
        if (string.IsNullOrWhiteSpace(nameColor))
            throw new DomainException($"NameColor cannot be empty");

        NameColor = nameColor;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void AddCharacterState(CharacterState? characterState)
    {
        if (characterState is null)
            throw new DomainException($"CharacterState cannot be null");
        
        if (_characterStates.Any(item => Equals(item, characterState)))
            throw new DomainException($"State {characterState.Name} already exists");
        
        _characterStates.Add(characterState);
    }

    public void RemoveCharacterStateById(Guid stateId)
    {
        if (stateId == Guid.Empty)
            throw new DomainException($"StateId {stateId} cannot be empty");

        var state = _characterStates.FirstOrDefault(item => item.Id == stateId);
        if (state is null)
            throw new DomainException($"StateId {stateId} doesn't exists");

        _characterStates.Remove(state);
    }
}
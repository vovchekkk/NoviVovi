using NoviVovi.Domain.Common;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Characters;

public class Character : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    private readonly List<CharacterState> _characterStates = new();

    public IReadOnlyList<CharacterState> CharacterStates => _characterStates;

    private Character(Guid id, string name, string? description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public static Character Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");

        return new Character(Guid.NewGuid(), name, description);
    }

    public static Character Rehydrate(Guid id, string name, IEnumerable<CharacterState> states,
        string? description = null)
    {
        var character = new Character(id, name, description);
        foreach (var state in states)
            character.AddState(state);
        return character;
    }

    public void AddState(CharacterState states)
    {
        if (_characterStates.Any(item => item.Id == states.Id))
            throw new DomainException($"Slide {states.Id} already exists");
        _characterStates.Add(states);
    }
}
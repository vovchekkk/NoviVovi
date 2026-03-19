using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Novels;

public class Novel : Entity
{
    public string Title { get; private set; }
    public Guid StartLabelId { get; private set; }

    private readonly List<Guid> _labelIds = new();
    private readonly List<Character> _characters = new();

    public IReadOnlyList<Guid> LabelIds => _labelIds.AsReadOnly();
    public IReadOnlyCollection<Character> Characters => _characters.AsReadOnly();

    private Novel(Guid id, string title, Guid startLabelId) : base(id)
    {
        Title = title;
        StartLabelId = startLabelId;
        _labelIds.Add(startLabelId);
    }

    public static Novel Create(string? title, Guid startLabelId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");

        if (startLabelId == Guid.Empty)
            throw new DomainException($"LabelId {startLabelId} cannot be empty");

        return new Novel(Guid.NewGuid(), title, startLabelId);
    }

    public static Novel Rehydrate(Guid id, string title, Guid startLabelId, IEnumerable<Guid> labelIds)
    {
        var novel = new Novel(id, title, startLabelId);
        foreach (var label in labelIds)
            novel.AddLabel(label);
        return novel;
    }

    public void AddLabel(Guid labelId)
    {
        if (labelId == Guid.Empty)
            throw new DomainException($"LabelId {labelId} cannot be empty");

        if (_labelIds.Any(item => item == labelId))
            throw new DomainException($"Label {labelId} already exists");

        _labelIds.Add(labelId);
    }

    public void RemoveLabel(Guid labelId)
    {
        if (labelId == Guid.Empty)
            throw new DomainException($"LabelId {labelId} cannot be empty");

        var label = _labelIds.FirstOrDefault(item => item == labelId);
        if (label == Guid.Empty)
            throw new DomainException($"Label {labelId} doesn't exists");

        _labelIds.Remove(labelId);
    }

    public void AddCharacter(Character character)
    {
        if (_characters.Any(item => Equals(item, character)))
            throw new DomainException($"Character {character.Id} already exists");

        _characters.Add(character);
    }

    public void RemoveCharacter(Guid characterId)
    {
        if (characterId == Guid.Empty)
            throw new DomainException($"CharacterId {characterId} cannot be empty");

        var character = _characters.FirstOrDefault(item => item.Id == characterId);
        if (character is null)
            throw new DomainException($"Character {characterId} doesn't exists");

        _characters.Remove(character);
    }
}
using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Novels;

public class Novel : Entity
{
    public string Title { get; private set; }
    public Label StartLabel { get; private set; }

    private readonly List<Label> _labels = new();
    private readonly List<Character> _characters = new();

    public IReadOnlyList<Label> Labels => _labels.AsReadOnly();
    public IReadOnlyCollection<Character> Characters => _characters.AsReadOnly();

    public Novel(Guid id, string title, Label startLabel) : base(id)
    {
        Title = title;
        StartLabel = startLabel;
        AddLabel(startLabel);
    }

    public static Novel Create(string? title, string startLabelName)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");
        
        var novelId = Guid.NewGuid();

        var startLabel = Label.Create(startLabelName, novelId);

        return new Novel(novelId, title, startLabel);
    }
    
    public void UpdateTitle(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");

        Title = title;
    }

    public void SetStartLabel(Label? label)
    {
        if (label is null)
            throw new DomainException("Start label cannot be null");
        
        if (!_labels.Contains(label))
            throw new DomainException($"Label {label.Id} does not belong to this novel");

        StartLabel = label;
    }

    public void AddLabel(Label? label)
    {
        if (label is null)
            throw new DomainException($"Label cannot be null");

        if (_labels.Any(item => Equals(item, label)))
            throw new DomainException($"Label {label.Name} already exists");

        _labels.Add(label);
    }

    public void RemoveLabelById(Guid labelId)
    {
        if (labelId == Guid.Empty)
            throw new DomainException($"LabelId {labelId} cannot be empty");

        var label = _labels.FirstOrDefault(item => item.Id == labelId);
        if (label is null)
            throw new DomainException($"Label {labelId} doesn't exists");

        _labels.Remove(label);
    }

    public void AddCharacter(Character? character)
    {
        if (character is null)
            throw new DomainException($"Character cannot be null");
        
        if (_characters.Any(item => Equals(item, character)))
            throw new DomainException($"Character {character.Name} already exists");

        _characters.Add(character);
    }

    public void RemoveCharacterById(Guid characterId)
    {
        if (characterId == Guid.Empty)
            throw new DomainException($"CharacterId {characterId} cannot be empty");

        var character = _characters.FirstOrDefault(item => item.Id == characterId);
        if (character is null)
            throw new DomainException($"Character {characterId} doesn't exists");

        _characters.Remove(character);
    }
}
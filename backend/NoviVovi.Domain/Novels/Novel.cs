using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Novels;

public class Novel : Entity
{
    public string Title { get; private set; }
    public Label StartLabel { get; private set; }
    private readonly Dictionary<Guid, Label> _labels = new();

    public IReadOnlyDictionary<Guid, Label> Labels => _labels.AsReadOnly();
    
    private Novel(Guid id, string title, Label startLabel) : base(id)
    {
        Title = title;
        StartLabel = startLabel;
    }

    public static Novel Create(string? title, Label startLabel)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");
        
        if (startLabel == null)
            throw new DomainException("StartLabel cannot be null");

        return new Novel(Guid.NewGuid(), title, startLabel);
    }

    public static Novel Rehydrate(Guid id, string title, Label startLabel, IEnumerable<Label> labels)
    {
        var novel = new Novel(id, title, startLabel);
        foreach (var label in labels)
            novel.AddLabel(label);
        return novel;
    }

    public void AddLabel(Label label)
    {
        if (_labels.Any(item => item.Key == label.Id))
            throw new DomainException($"Label {label.Id} already exists");
        _labels[label.Id] = label;
    }

    public void RemoveLabel(Guid id)
    {
        var label = _labels.FirstOrDefault(item => item.Key == id).Value;
        if (label == null)
            throw new DomainException($"Label {id} does not exist");
        _labels.Remove(id);
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Label cannot be empty");
        Title = title;
    }
}
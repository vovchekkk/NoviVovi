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

    public IReadOnlyList<Guid> LabelIds => _labelIds.AsReadOnly();
    
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
        if (_labelIds.Any(item => item == labelId))
            throw new DomainException($"Label {labelId} already exists");
        _labelIds.Add(labelId);
    }

    public void RemoveLabel(Guid id)
    {
        var label = _labelIds.FirstOrDefault(item => item == id);
        if (label == Guid.Empty)
            throw new DomainException($"LabelId {id} cannot be empty");
        _labelIds.Remove(id);
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Label cannot be empty");
        Title = title;
    }
}
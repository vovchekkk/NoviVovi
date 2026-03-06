using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Novels;

public class Novel : Entity
{
    private readonly List<Label> _labels = new();
    public string Title { get; private set; }

    public IReadOnlyList<Label> Labels => _labels.AsReadOnly();
    
    private Novel(Guid id, string title) : base(id)
    {
        Title = title;
    }

    public static Novel Create(string? title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");

        return new Novel(Guid.NewGuid(), title);
    }

    public static Novel Rehydrate(Guid id, string title, IEnumerable<Label> labels)
    {
        var novel = new Novel(id, title);
        foreach (var label in labels)
            novel.AddLabel(label);
        return novel;
    }

    public void AddLabel(Label label)
    {
        if (_labels.Any(item => item.Id == label.Id))
            throw new DomainException($"Slide {label.Id} already exists");
        _labels.Add(label);
    }

    public void RemoveLabel(Guid id)
    {
        var slide = _labels.FirstOrDefault(item => item.Id == id);
        if (slide == null)
            throw new DomainException($"Slide {id} does not exist");
        _labels.Remove(slide);
    }

    public void UpdateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title cannot be empty");
        Title = title;
    }
}
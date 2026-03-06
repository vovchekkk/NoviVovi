using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.Labels;

public class Label : Entity
{
    public string Name { get; private set; }
    private readonly List<SceneStep> _steps = new();

    public IReadOnlyList<SceneStep> Steps => _steps;

    private Label(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public static Label Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");

        return new Label(Guid.NewGuid(), name);
    }

    public static Label Rehydrate(Guid id, string name, IEnumerable<SceneStep> steps)
    {
        var label = new Label(id, name);
        foreach (var step in steps)
            label.AddStep(step);
        return label;
    }

    public void AddStep(SceneStep step)
    {
        if (_steps.Any(item => item.Id == step.Id))
            throw new DomainException($"Slide {step.Id} already exists");
        _steps.Add(step);
    }
}
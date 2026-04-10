using NoviVovi.Domain.Common;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Domain.Labels;

public class Label : Entity
{
    public string Name { get; private set; }
    private readonly List<Step> _steps = new();

    public IReadOnlyList<Step> Steps => _steps;

    public Label(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public static Label Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");

        return new Label(Guid.NewGuid(), name);
    }

    public void AddStep(Step step)
    {
        if (step is null)
            throw new DomainException($"Step cannot be null");
        
        if (_steps.Any(item => Equals(item, step)))
            throw new DomainException($"Step {step.Id} already exists");
        
        _steps.Add(step);
    }
}
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Domain.Labels;

public class Label : Entity
{
    public string Name { get; private set; }
    public Guid NovelId { get; private set; }
    private readonly List<Step> _steps = new();

    public IReadOnlyList<Step> Steps => _steps.AsReadOnly();

    private Label(Guid id, string name, Guid novelId) : base(id)
    {
        Name = name;
        NovelId = novelId;
    }

    public static Label Create(string? name, Guid novelId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");
        
        if (novelId == Guid.Empty)
            throw new DomainException($"NovelId cannot be empty");

        return new Label(Guid.NewGuid(), name, novelId);
    }
    
    public IEnumerable<Step> GetStepsUntil(Guid stepId)
    {
        foreach (var step in _steps)
        {
            yield return step;
            if (step.Id == stepId) yield break;
        }
    }

    public void UpdateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Name cannot be empty");

        Name = name;
    }

    public void AddStep(Step? step)
    {
        if (step is null)
            throw new DomainException($"Step cannot be null");
        
        if (_steps.Any(item => Equals(item, step)))
            throw new DomainException($"Step {step.Id} already exists");
        
        _steps.Add(step);
    }

    public void RemoveStepById(Guid stepId)
    {
        if (stepId == Guid.Empty)
            throw new DomainException($"StepId {stepId} cannot be empty");

        var step = _steps.FirstOrDefault(item => item.Id == stepId);
        if (step is null)
            throw new DomainException($"StepId {stepId} doesn't exists");

        _steps.Remove(step);
    }
}
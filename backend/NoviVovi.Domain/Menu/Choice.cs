using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Menu;

public class Choice : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? Text { get; private set; }
    public ChoiceTransition Transition { get; private set; }
    
    private Choice(Guid id, string name, string? description, string? text, ChoiceTransition transition) : base(id)
    {
        Name = name;
        Description = description;
        Text = text;
        Transition = transition;
    }

    public static Choice Create(
        ChoiceTransition transition,
        string? name,
        string? description = null,
        string? text = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException($"Name cannot be empty");
        
        if (transition is null)
            throw new DomainException($"Transition cannot be null");

        return new Choice(Guid.NewGuid(), name, description, text, transition);
    }
}
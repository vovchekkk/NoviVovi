using NoviVovi.Domain.Common;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Menu;

public class Choice : Entity
{
    public string Text { get; private set; }
    public ChoiceTransition Transition { get; private set; }

    public Choice(Guid id, string text, ChoiceTransition transition) : base(id)
    {
        Text = text;
        Transition = transition;
    }

    public static Choice Create(
        string text,
        ChoiceTransition transition
    )
    {
        if (transition is null)
            throw new DomainException($"Transition cannot be null");

        return new Choice(Guid.NewGuid(), text, transition);
    }
}
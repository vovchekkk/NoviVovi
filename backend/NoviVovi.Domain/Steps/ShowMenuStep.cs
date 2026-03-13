using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowMenuStep(Guid id, Menu.Menu menu, ChoiceTransition transition) : Step(id, transition)
{
    public Menu.Menu Menu { get; } = menu;
}
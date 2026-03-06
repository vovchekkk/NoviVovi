using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowMenuStep(Guid id, Menu.Menu menu, ChoiceTransition transition) : Step(id, transition)
{
    public Menu.Menu Menu { get; } = menu;
}
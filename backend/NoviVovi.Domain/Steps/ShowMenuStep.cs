using NoviVovi.Domain.Common;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowMenuStep : Step
{
    public Menu.Menu Menu { get; }
    
    private ShowMenuStep(Guid id, Menu.Menu menu, Transition transition) : base(id, transition)
    {
        Menu = menu;
    }

    public static ShowMenuStep Create(Menu.Menu menu)
    {
        return new ShowMenuStep(Guid.NewGuid(), menu, NextStepTransition.Create());
    }

    public static ShowMenuStep Rehydrate(Guid id, Menu.Menu menu, Transition transition)
    {
        return new ShowMenuStep(id, menu, transition);
    }
}
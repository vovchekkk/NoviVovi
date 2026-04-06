using NoviVovi.Domain.Common;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowMenuStep : Step
{
    public Menu.Menu Menu { get; }
    
    private ShowMenuStep(Guid id, Menu.Menu menu, NextStepTransition transition) : base(id, transition)
    {
        Menu = menu;
    }

    public static ShowMenuStep Create(Menu.Menu? menu)
    {
        if (menu is null)
            throw new DomainException($"Menu cannot be null");
        
        return new ShowMenuStep(Guid.NewGuid(), menu, NextStepTransition.Create());
    }
    
    public new NextStepTransition Transition => (NextStepTransition)base.Transition;
}
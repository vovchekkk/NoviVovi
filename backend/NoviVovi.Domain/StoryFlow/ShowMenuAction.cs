namespace NoviVovi.Domain.StoryFlow;

public class ShowMenuAction : NextAction
{
    public Menu.Menu Menu { get; }

    public ShowMenuAction(Menu.Menu menu)
    {
        Menu = menu;
    }
}
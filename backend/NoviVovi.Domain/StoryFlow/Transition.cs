namespace NoviVovi.Domain.StoryFlow;

public class Transition
{
    public NextAction Action { get; }

    public Transition(NextAction action)
    {
        Action = action;
    }
}
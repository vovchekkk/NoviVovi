namespace NoviVovi.Domain.Transitions;

public sealed class NextStepTransition : Transition
{
    private NextStepTransition()
    {
    }

    public static NextStepTransition Create()
    {
        return new NextStepTransition();
    }
}
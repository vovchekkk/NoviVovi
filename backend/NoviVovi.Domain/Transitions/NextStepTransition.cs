namespace NoviVovi.Domain.Transitions;

public sealed class NextStepTransition : Transition
{
    private NextStepTransition(Guid id) : base(id)
    {
    }

    public static NextStepTransition Create()
    {
        return new NextStepTransition(Guid.NewGuid());
    }
}
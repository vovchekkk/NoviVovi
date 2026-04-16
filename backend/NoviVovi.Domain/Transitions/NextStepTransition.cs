namespace NoviVovi.Domain.Transitions;

public sealed class NextStepTransition : Transition
{
    public NextStepTransition()
    {
    }

    public static NextStepTransition Create()
    {
        return new NextStepTransition();
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield break;
    }
}
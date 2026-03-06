using NoviVovi.Domain.Scene;

namespace NoviVovi.Domain.StoryFlow;

public class NextStepAction : NextAction
{
    public SceneStep NextStep { get; }

    public NextStepAction(SceneStep nextStep)
    {
        NextStep = nextStep;
    }
}
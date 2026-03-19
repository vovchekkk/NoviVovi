using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowBackgroundStep : Step
{
    public BackgroundObject Background { get; }
    public Transform Transform { get; }

    private ShowBackgroundStep(
        Guid id,
        BackgroundObject background,
        Transform transform,
        Transition transition
    ) : base(id, transition)
    {
        Background = background;
        Transform = transform;
    }

    public static ShowBackgroundStep Create(
        BackgroundObject background,
        Transform transform
    )
    {
        return new ShowBackgroundStep(Guid.NewGuid(), background, transform, NextStepTransition.Create());
    }

    public static ShowBackgroundStep Rehydrate(
        Guid id,
        BackgroundObject background,
        Transform transform,
        Transition transition
    )
    {
        return new ShowBackgroundStep(id, background, transform, transition);
    }
}
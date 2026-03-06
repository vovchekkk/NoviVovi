using NoviVovi.Domain.Images;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps.Transitions;

namespace NoviVovi.Domain.Steps;

public class ShowBackgroundStep(
    Guid id,
    Image background,
    Transform transform,
    NextStepTransition transition)
    : Step(id, transition)
{
    public Image Background { get; } = background;
    public Transform Transform { get; } = transform;
}
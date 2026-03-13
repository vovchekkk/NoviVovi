using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Domain.Steps;

public class JumpStep(Guid id, Label label, JumpTransition transition) : Step(id, transition)
{
    public Label Label { get; } = label;
}
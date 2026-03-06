using NoviVovi.Domain.Labels;

namespace NoviVovi.Domain.StoryFlow;

public class JumpToLabelAction : NextAction
{
    public Label TargetLabel { get; }

    public JumpToLabelAction(Label label)
    {
        TargetLabel = label;
    }
}
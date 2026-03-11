using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Steps.Transitions;

namespace NoviVovi.Application.Preview.Contracts;

public class ScenePlayer(Novel novel, SceneState state)
{
    private readonly Novel _novel = novel;

    private Label _currentLabel = novel.StartLabel;
    private int _stepIndex;

    public void ExecuteNext()
    {
        var step = _currentLabel.Steps[_stepIndex];

        switch (step)
        {
            case JumpStep s:
                return;

            case ShowMenuStep s:
                state.ShowMenu(s);
                return;

            case ShowCharacterStep s:
                state.ShowCharacter(s);
                break;

            case HideCharacterStep s:
                state.HideCharacter(s);
                break;

            case ShowBackgroundStep s:
                state.ShowBackground(s);
                break;

            case ShowReplicaStep s:
                state.ShowReplica(s);
                break;
        }

        ExecuteTransition(step.Transition);
    }
    
    public void SelectChoice(Choice choice)
    {
        if (choice == null)
            throw new ArgumentNullException(nameof(choice));

        state.HideMenu();

        ExecuteTransition(choice.Transition);
    }
    
    private void ExecuteTransition(StepTransition transition)
    {
        switch (transition)
        {
            case NextStepTransition:
                _stepIndex++;
                break;

            case JumpTransition jump:
                _currentLabel = jump.TargetLabel;
                _stepIndex = 0;
                state.Reset();
                break;
            
            case ChoiceTransition choice:
                _currentLabel = choice.TargetLabel;
                _stepIndex = 0;
                state.Reset();
                break;
            
            default:
                throw new InvalidOperationException("Unknown transition");
        }
    }
}
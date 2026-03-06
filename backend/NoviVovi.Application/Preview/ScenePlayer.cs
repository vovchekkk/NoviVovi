using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Steps.Transitions;

namespace NoviVovi.Application.Preview;

public class ScenePlayer(Novel novel, SceneState state)
{
    private readonly Novel _novel = novel;

    private Label _currentLabel = novel.StartLabel;
    private int _stepIndex;

    public void Execute()
    {
        var step = _currentLabel.Steps[_stepIndex];

        switch (step)
        {
            case JumpStep s:
                ExecuteJump(s);
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

            case SayStep s:
                state.Say(s);
                break;
        }

        ExecuteTransition(step.Transition);
    }
    
    private void ExecuteJump(JumpStep step)
    {
        _currentLabel = step.Label;
        _stepIndex = 0;
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
                break;
            
            case ChoiceTransition choice:
                _currentLabel = choice.TargetLabel;
                _stepIndex = 0;
                break;
            
            default:
                throw new InvalidOperationException("Unknown transition");
        }
    }
}
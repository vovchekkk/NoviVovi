using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Application.Preview.Models;

public class ScenePlayer(SceneState state)
{
    private Guid _currentLabelId;
    private int _stepIndex;

    public void Initialize(Guid startLabelId)
    {
        _currentLabelId = startLabelId;
        _stepIndex = 0;
    }

    public void RestoreState(Guid labelId, int stepIndex)
    {
        _currentLabelId = labelId;
        _stepIndex = stepIndex;
    }

    public async Task ExecuteNextAsync(ILabelRepository repository)
    {
        var label = await repository.GetByIdAsync(_currentLabelId);
        if (label == null)
            throw new NotFoundException("Label not found");

        while (_stepIndex < label.Steps.Count)
        {
            var step = label.Steps[_stepIndex];
            var isBlocking = false;

            switch (step)
            {
                case ShowMenuStep s:
                    state.ShowMenu(s);
                    isBlocking = true;
                    break;

                case ShowReplicaStep s:
                    state.ShowReplica(s);
                    isBlocking = true;
                    break;

                case ShowCharacterStep s:
                    state.ShowCharacter(s);
                    break;

                case HideCharacterStep s:
                    state.HideCharacter(s);
                    break;

                case ShowBackgroundStep s:
                    state.ShowBackground(s);
                    break;
            }

            if (step is not ShowMenuStep)
            {
                ApplyTransition(step.Transition);
            }

            if (isBlocking)
                break;

            if (_currentLabelId != label.Id)
                break;
        }
    }

    public void SelectChoice(Guid choiceId)
    {
        if (state.Menu == null)
            throw new InvalidOperationException("No menu is currently active.");

        var choice = state.Menu.Choices.FirstOrDefault(c => c.Id == choiceId);
        if (choice == null)
            throw new InvalidOperationException("Choice not found in the current menu.");

        state.HideMenu();
        ApplyTransition(choice.Transition);
    }

    private void ApplyTransition(Transition transition)
    {
        switch (transition)
        {
            case NextStepTransition:
                _stepIndex++;
                break;

            case JumpTransition jump:
                _currentLabelId = jump.TargetLabelId;
                _stepIndex = 0;
                state.Reset();
                break;

            case ChoiceTransition choice:
                _currentLabelId = choice.TargetLabelId;
                _stepIndex = 0;
                state.Reset();
                break;

            default:
                throw new InvalidOperationException("Unknown transition");
        }
    }
}
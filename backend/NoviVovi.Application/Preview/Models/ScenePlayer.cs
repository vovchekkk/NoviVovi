using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;

namespace NoviVovi.Application.Preview.Models;

public class ScenePlayer(SceneState state, Label startLabel)
{
    private Label? CurrentLabel { get; set; } = startLabel;
    private int StepIndex { get; set; }

    public void RestoreState(Label label, int stepIndex)
    {
        CurrentLabel = label;
        StepIndex = stepIndex;
    }

    public async Task ExecuteNextAsync(ILabelRepository repository)
    {
        if (CurrentLabel == null)
            throw new NotFoundException("Label not found");

        while (StepIndex < CurrentLabel.Steps.Count)
        {
            var step = CurrentLabel.Steps[StepIndex];
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
                StepIndex++;
                break;

            case JumpTransition jump:
                CurrentLabel = jump.TargetLabel;
                StepIndex = 0;
                state.Reset();
                break;

            case ChoiceTransition choice:
                CurrentLabel = choice.TargetLabel;
                StepIndex = 0;
                state.Reset();
                break;

            default:
                throw new InvalidOperationException("Unknown transition");
        }
    }
}
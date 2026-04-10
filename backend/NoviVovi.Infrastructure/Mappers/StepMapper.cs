using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;
using NoviVovi.Infrastructure.DatabaseObjects.Enums;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;


[Mapper]
public partial class StepMapper
{
    public StepDbO ToDbO(HideCharacterStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.HideCharacter.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            CharacterId = step.Character.Id //TODO! убить владимира
        };
        return new StepDbO();
    }

    public StepDbO ToDbo(JumpStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.Jump.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            NextLabelId = step.Transition.TargetLabel.Id
        };
        return res;
    }

    public StepDbO ToDbo(ShowBackgroundStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowBackground.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            BgId = step.BackgroundObject.Id
        };
        return res;
    }

    public StepDbO ToDbo(ShowCharacterStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowCharacter.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            CharacterId = step.CharacterObject.Id
        };
        return res;
    }

    public StepDbO ToDbo(ShowMenuStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowMenu.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            MenuId = step.Menu.Id
        };
        return res;
    }

    public StepDbO ToDbo(ShowReplicaStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowReplica.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            ReplicaId = step.Replica.Id
        };
        return res;
    }

    // public HideCharacterStep ToHideCharacterStep(StepDbO step)
    // {
    //     
    // }
    public Step ToDomain(StepDbO dbo)
    {
        throw new NotImplementedException();
    }

    public StepDbO ToDbO(Step step, Guid labelId, int stepOrder)
    {
        throw new NotImplementedException();
    }
}
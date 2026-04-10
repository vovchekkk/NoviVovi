using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;
using NoviVovi.Infrastructure.DatabaseObjects.Enums;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;


[Mapper]
public partial class StepMapper(
    LabelMapper labelMapper,
    ImageMapper imageMapper,
    CharacterMapper characterMapper,
    MenuMapper menuMapper,
    ReplicaMapper replicaMapper)
{
    private StepDbO ToDbO(HideCharacterStep step, Guid labelId, int stepOrder)
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

    public StepDbO ToDbO(JumpStep step, Guid labelId, Guid novelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.Jump.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            NextLabelId = step.Transition.TargetLabel.Id
        };
        var label = labelMapper.ToDbO(step.Transition.TargetLabel, novelId);
        res.NextLabel = label;
        return res;
    }

    public StepDbO ToDbO(ShowBackgroundStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowBackground.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            BackgroundId = step.BackgroundObject.Id
        };
        return res;
    }

    public StepDbO ToDbO(ShowCharacterStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowCharacter.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            CharacterId = step.CharacterObject.Id
        };
        var character = characterMapper.ToDbO(step.CharacterObject);
        res.Character = character;
        return res;
    }

    public StepDbO ToDbO(ShowMenuStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowMenu.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            MenuId = step.Menu.Id
        };
        var menu = menuMapper.ToDbO(step.Menu);
        res.Menu = menu;
        return res;
    }

    public StepDbO ToDbO(ShowReplicaStep step, Guid labelId, Guid novelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowReplica.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            ReplicaId = step.Replica.Id
        };
        var replica = replicaMapper.ToDbO(step.Replica, novelId);
        res.Replica = replica;
        return res;
    }

    public HideCharacterStep ToHideCharacterStep(StepDbO step)
    {
        throw new NotImplementedException(); //уберу когда ребейзнусь или смерджусь
    }

    public JumpStep ToJumpStep(StepDbO step)
    {
        if (step.NextLabelId == null || step.NextLabel == null)
            throw new ArgumentException("Invalid JumpStep StepDbO, not a jump, not a step");
        var res = new JumpStep(step.Id, new JumpTransition(Guid.Empty, labelMapper.ToDomain(step.NextLabel)));
        return res;
    }

    public ShowBackgroundStep ToShowBackgroundStep(StepDbO step)
    {
        if(step.BackgroundId == null || step.Background == null)
            throw new ArgumentException("Invalid BackgroundStep StepDbO");
        var res = new ShowBackgroundStep(step.Id, imageMapper.ToDbO(step.Background),
            new NextStepTransition(Guid.Empty));
        return res;
    }

    public ShowCharacterStep ToShowCharacterStep(StepDbO step)
    {
        if(step.CharacterId == null || step.Character == null)
            throw new ArgumentException("Invalid CharacterStep StepDbO");
        var res = new ShowCharacterStep(step.Id, characterMapper.ToDomain(step), new NextStepTransition(Guid.Empty));
        return res;
    }
    
    public ShowMenuStep ToShowMenuStep(StepDbO step)
    {
        if(step.MenuId == null || step.Menu == null)
            throw new ArgumentException("Invalid MenuStep StepDbO");
        var res = new ShowMenuStep(step.Id, menuMapper.ToDomain(step.Menu), new NextStepTransition(Guid.Empty));
        return res;
    }

    public ShowReplicaStep ToShowReplicaStep(StepDbO step)
    {
        if(step.ReplicaId == null || step.Replica == null)
            throw new ArgumentException("Invalid ReplicaStep StepDbO");
        var res = new ShowReplicaStep(step.Id, replicaMapper.ToDomain(step.Replica), new NextStepTransition(Guid.Empty));
        return res;
    }
    
    public Step ToDomain(StepDbO dbo)
    {
        var type = dbo.StepType.ToStepType();
        switch (type)
        {
            case StepType.HideCharacter:
                return ToHideCharacterStep(dbo);
            case StepType.Jump:
                return ToJumpStep(dbo);
            case StepType.ShowBackground:
                return ToShowBackgroundStep(dbo);
            case StepType.ShowMenu:
                return ToShowMenuStep(dbo);
            case StepType.ShowReplica:
                return ToShowReplicaStep(dbo);
            default:
                throw new ArgumentOutOfRangeException($"Unknown step type {dbo.StepType}");
        }
    }

    public StepDbO ToDbO(Step step, Guid labelId, Guid novelId, int stepOrder)
    {
        var type = typeof(Step);
        if(type == typeof(HideCharacterStep))
            return ToDbO((HideCharacterStep)step, labelId, stepOrder);
        if(type == typeof(JumpStep))
            return ToDbO((JumpStep)step, labelId, novelId, stepOrder);
        if(type == typeof(ShowBackgroundStep))
            return ToDbO((ShowBackgroundStep)step, labelId, stepOrder);
        if(type == typeof(ShowMenuStep))
            return ToDbO((ShowMenuStep)step, labelId, stepOrder);
        if(type == typeof(ShowReplicaStep))
            return ToDbO((ShowReplicaStep)step, labelId, novelId, stepOrder);
        throw new ArgumentException("Unsupported step type");
    }
}
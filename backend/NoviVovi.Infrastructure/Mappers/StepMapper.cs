using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Enums;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class StepMapper(
    Lazy<LabelMapper> labelMapper,
    ImageMapper imageMapper,
    CharacterMapper characterMapper,
    Lazy<MenuMapper> menuMapper,
    ReplicaMapper replicaMapper
)
{
    private StepDbO ToDbO(HideCharacterStep step, Guid labelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.HideCharacter.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            CharacterId = step.Character.Id
        };
        return new StepDbO();
    }
    
    

    public StepDbO ToDbO(JumpStep step, Guid labelId, Guid novelId, int stepOrder, MappingContext ctx)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.Jump.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            NextLabelId = step.Transition.TargetLabel.Id,
            NextLabel = labelMapper.Value.ToDbO(step.Transition.TargetLabel, ctx)
        };

        return res;
    }

    public StepDbO ToDbO(ShowBackgroundStep step, Guid labelId, Guid novelId, int stepOrder)
    {
        return new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowBackground.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            BackgroundId = step.BackgroundObject.Id,
            Background = imageMapper.ToDbO(step.BackgroundObject, novelId)
        };
    }

    public StepDbO ToDbO(ShowCharacterStep step, Guid labelId, Guid novelId, int stepOrder)
    {
        var res = new StepDbO
        {
            Id = step.Id,
            StepType = StepType.ShowCharacter.ToStepTypeString(),
            LabelId = labelId,
            StepOrder = stepOrder,
            CharacterId = step.CharacterObject.Id
        };

        res.Character = characterMapper.ToDbO(step.CharacterObject, novelId);
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

        res.Menu = menuMapper.Value.ToDbO(step.Menu);
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

        res.Replica = replicaMapper.ToDbO(step.Replica, novelId);
        return res;
    }

    public HideCharacterStep ToHideCharacterStep(StepDbO step)
    {
        if (step.CharacterId == null || step.Character == null)
            throw new ArgumentException("Invalid step character");
        var res = new HideCharacterStep(step.Id, characterMapper.ToDomain(step.Character.Character),
            new NextStepTransition());
        return res;
    }

    public JumpStep ToJumpStep(StepDbO step, MappingContext ctx)
    {
        if (step.NextLabel == null)
            throw new ArgumentException();

        return new JumpStep(
            step.Id,
            new JumpTransition(labelMapper.Value.ToDomain(step.NextLabel, ctx))
        );
    }

    public ShowBackgroundStep ToShowBackgroundStep(StepDbO step)
    {
        if (step.Background == null)
            throw new ArgumentException();

        return new ShowBackgroundStep(
            step.Id,
            imageMapper.ToDomain(step.Background),
            new NextStepTransition()
        );
    }

    public ShowCharacterStep ToShowCharacterStep(StepDbO step)
    {
        if (step.Character == null)
            throw new ArgumentException();

        return new ShowCharacterStep(
            step.Id,
            characterMapper.ToDomain(step.Character),
            new NextStepTransition()
        );
    }

    public ShowMenuStep ToShowMenuStep(StepDbO step)
    {
        if (step.Menu == null)
            throw new ArgumentException();

        return new ShowMenuStep(
            step.Id,
            menuMapper.Value.ToDomain(step.Menu),
            new NextStepTransition()
        );
    }

    public ShowReplicaStep ToShowReplicaStep(StepDbO step)
    {
        if (step.Replica == null)
            throw new ArgumentException($"ShowReplicaStep (ID: {step.Id}) не имеет связанной реплики в БД");

        return new ShowReplicaStep(
            step.Id,
            replicaMapper.ToDomain(step.Replica),
            new NextStepTransition()
        );
    }

    public Step ToDomain(StepDbO dbo, MappingContext ctx)
    {
        var type = dbo.StepType.ToStepType();
        switch (type)
        {
            case StepType.HideCharacter:
                return ToHideCharacterStep(dbo);
            case StepType.Jump:
                return ToJumpStep(dbo, ctx);
            case StepType.ShowBackground:
                return ToShowBackgroundStep(dbo);
            case StepType.ShowMenu:
                return ToShowMenuStep(dbo);
            case StepType.ShowReplica:
                return ToShowReplicaStep(dbo);
            case StepType.ShowCharacter:
                return ToShowCharacterStep(dbo);
            default:
                throw new ArgumentOutOfRangeException($"Unknown step type {dbo.StepType}");
        }
    }

    public StepDbO ToDbO(Step step, Guid labelId, Guid novelId, int stepOrder, MappingContext ctx)
    {
        var type = step.GetType();
        if (type == typeof(HideCharacterStep))
            return ToDbO((HideCharacterStep)step, labelId, stepOrder);
        if (type == typeof(JumpStep))
            return ToDbO((JumpStep)step, labelId, novelId, stepOrder, ctx);
        if (type == typeof(ShowBackgroundStep))
            return ToDbO((ShowBackgroundStep)step, labelId, novelId, stepOrder);
        if (type == typeof(ShowMenuStep))
            return ToDbO((ShowMenuStep)step, labelId, stepOrder);
        if (type == typeof(ShowReplicaStep))
            return ToDbO((ShowReplicaStep)step, labelId, novelId, stepOrder);
        if (type == typeof(ShowCharacterStep))
            return ToDbO((ShowCharacterStep)step, labelId, novelId, stepOrder);
        throw new ArgumentException("Unsupported step type");
    }
}
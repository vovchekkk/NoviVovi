using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Menu.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Services;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Mappers;

/// <summary>
/// Maps Domain Step to RenPy statement.
/// </summary>
public class StepToRenPyMapper(
    RenPyIdentifierGenerator idGenerator,
    TransformToRenPyMapper transformMapper
)
{
    public RenPyStatement Map(Step step)
    {
        return step switch
        {
            HideCharacterStep hideStep => new RenPyHideCharacterStatement(
                idGenerator.GenerateForCharacter(hideStep.Character.Id)
            ),
            
            JumpStep jumpStep => new RenPyJumpStatement(
                idGenerator.GenerateForLabel(jumpStep.Transition.TargetLabel.Id)
            ),
            
            ShowBackgroundStep bgStep => new RenPySceneStatement(
                idGenerator.GenerateForImage(bgStep.BackgroundObject.Image.Id)
            ),
            
            ShowCharacterStep charStep => new RenPyShowCharacterStatement(
                idGenerator.GenerateForCharacter(charStep.CharacterObject.Character.Id),
                idGenerator.GenerateForCharacterState(charStep.CharacterObject.State.Image.Id),
                transformMapper.Map(charStep.CharacterObject.Transform)
            ),
            
            ShowMenuStep menuStep => new RenPyMenu(
                menuStep.Menu.Choices.Select(choice => new RenPyChoice(
                    RenPyHelper.EscapeString(choice.Text)
                )).ToList()
            ),
            
            ShowReplicaStep replicaStep => new RenPyReplicaStatement(
                idGenerator.GenerateForCharacter(replicaStep.Replica.Speaker.Id),
                RenPyHelper.EscapeString(replicaStep.Replica.Text)
            ),

            _ => throw new NotSupportedException($"Step type {step.GetType().Name} is not supported for RenPy export")
        };
    }

    private RenPyTransform? ToPosition(NoviVovi.Domain.Scene.Transform? transform)
    {
        if (transform == null)
            return null;

        return new RenPyTransform(
            (int)transform.Position.X,
            (int)transform.Position.Y,
            transform.Scale,
            transform.Rotation,
            transform.ZIndex
        );
    }
}
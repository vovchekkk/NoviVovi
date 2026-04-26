using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Menu.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;

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
                idGenerator.GenerateForImage(bgStep.BackgroundObject.Image.Id),
                transformMapper.Map(bgStep.BackgroundObject.Transform, bgStep.BackgroundObject.Image.Size)
            ),
            
            ShowCharacterStep charStep => new RenPyShowCharacterStatement(
                idGenerator.GenerateForCharacter(charStep.CharacterObject.Character.Id),
                idGenerator.GenerateForCharacterState(charStep.CharacterObject.State.Id),
                transformMapper.Map(charStep.CharacterObject.Transform, charStep.CharacterObject.State.Image.Size)
            ),
            
            ShowMenuStep menuStep => new RenPyShowMenuStatement(
                menuStep.Menu.Choices.Select(choice => new RenPyChoice(
                    RenPyHelper.EscapeString(choice.Text),
                    idGenerator.GenerateForLabel(choice.Transition.TargetLabel.Id)
                )).ToList()
            ),
            
            ShowReplicaStep replicaStep => new RenPyReplicaStatement(
                idGenerator.GenerateForCharacter(replicaStep.Replica.Speaker.Id),
                RenPyHelper.EscapeString(replicaStep.Replica.Text)
            ),

            _ => throw new NotSupportedException($"Step type {step.GetType().Name} is not supported for RenPy export")
        };
    }
}
using System.Text;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Menu.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Abstractions;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services;

/// <summary>
/// Renders RenPy statements to their string representation.
/// Follows Single Responsibility Principle: only handles statement rendering.
/// Follows Open/Closed Principle: new statement types can be added via extension methods or strategy pattern.
/// </summary>
public class RenPyStatementRenderer : IRenPyStatementRenderer
{
    public string Render(RenPyStatement statement, int indentLevel = 1)
    {
        var indent = new string(' ', indentLevel * 4);
        
        return statement switch
        {
            RenPySceneStatement scene => 
                $"{indent}scene {scene.BackgroundName}",
            
            RenPyShowCharacterStatement showChar => 
                RenderShowCharacter(showChar, indent),
            
            RenPyHideCharacterStatement hideChar => 
                $"{indent}hide {hideChar.ImageName}",
            
            RenPyReplicaStatement replica => 
                $"{indent}{replica.CharacterVar} \"{replica.Text}\"",
            
            RenPyJumpStatement jump => 
                $"{indent}jump {jump.TargetLabel}",
            
            RenPyReturnStatement => 
                $"{indent}return",
            
            RenPyMenu menu => 
                RenderMenu(menu, indentLevel),
            
            _ => $"{indent}# Unknown statement: {statement.GetType().Name}"
        };
    }

    private string RenderShowCharacter(RenPyShowCharacterStatement showChar, string indent)
    {
        if (showChar.Transform != null)
        {
            var position = FormatTransform(showChar.Transform);
            return $"{indent}show {showChar.CharacterName} {showChar.CharacterStateName} at {position}";
        }
        
        return $"{indent}show {showChar.CharacterName} {showChar.CharacterStateName}";
    }

    private string RenderMenu(RenPyMenu menu, int indentLevel)
    {
        var sb = new StringBuilder();
        var indent = new string(' ', indentLevel * 4);
        var choiceIndent = new string(' ', (indentLevel + 1) * 4);

        sb.AppendLine($"{indent}menu:");
        
        foreach (var choice in menu.Choices)
        {
            sb.AppendLine($"{choiceIndent}\"{choice.Text}\":");
            // TODO: Add choice actions when RenPyChoice model is extended
        }

        return sb.ToString().TrimEnd();
    }

    private string FormatTransform(RenPyTransform transform)
    {
        // For now, use predefined positions based on X coordinate
        // TODO: Generate custom ATL transforms for complex positioning
        if (transform.XPos < 400)
            return "left";
        if (transform.XPos > 880)
            return "right";
        return "center";
    }
}

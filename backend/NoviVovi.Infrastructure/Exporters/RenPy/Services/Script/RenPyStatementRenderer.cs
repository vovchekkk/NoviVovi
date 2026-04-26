using System.Text;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Menu.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;

namespace NoviVovi.Infrastructure.Exporters.RenPy.Services.Script;

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
                RenderScene(scene, indent),
            
            RenPyShowCharacterStatement showChar => 
                RenderShowCharacter(showChar, indent),
            
            RenPyHideCharacterStatement hideChar => 
                $"{indent}hide {hideChar.CharacterName}",
            
            RenPyReplicaStatement replica => 
                $"{indent}{replica.CharacterVar} \"{replica.Text}\"",
            
            RenPyJumpStatement jump => 
                $"{indent}jump {jump.TargetLabel}",
            
            RenPyReturnStatement => 
                $"{indent}return",
            
            RenPyShowMenuStatement menu => 
                RenderMenu(menu, indentLevel),
            
            _ => $"{indent}# Unknown statement: {statement.GetType().Name}"
        };
    }

    private string RenderScene(RenPySceneStatement scene, string indent)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{indent}scene {scene.BackgroundName}:");
        sb.AppendLine($"{indent}    xpos {FormatPosition(scene.Transform.XPos)}");
        sb.AppendLine($"{indent}    ypos {FormatPosition(scene.Transform.YPos)}");
        sb.AppendLine($"{indent}    zoom {scene.Transform.Zoom.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
        sb.AppendLine($"{indent}    xzoom {scene.Transform.XZoom.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
        sb.AppendLine($"{indent}    yzoom {scene.Transform.YZoom.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
        
        if (scene.Transform.Rotate != 0)
            sb.AppendLine($"{indent}    rotate {scene.Transform.Rotate.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
        
        sb.AppendLine($"{indent}    zorder {scene.Transform.ZOrder}");
        
        return sb.ToString().TrimEnd();
    }

    private string RenderShowCharacter(RenPyShowCharacterStatement showChar, string indent)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{indent}show {showChar.CharacterName} {showChar.CharacterStateName}:");
        sb.AppendLine($"{indent}    xpos {FormatPosition(showChar.Transform.XPos)}");
        sb.AppendLine($"{indent}    ypos {FormatPosition(showChar.Transform.YPos)}");
        sb.AppendLine($"{indent}    zoom {showChar.Transform.Zoom.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
        sb.AppendLine($"{indent}    xzoom {showChar.Transform.XZoom.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
        sb.AppendLine($"{indent}    yzoom {showChar.Transform.YZoom.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
        
        if (showChar.Transform.Rotate != 0)
            sb.AppendLine($"{indent}    rotate {showChar.Transform.Rotate.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}");
        
        sb.AppendLine($"{indent}    zorder {showChar.Transform.ZOrder}");
        
        return sb.ToString().TrimEnd();
    }

    private string RenderMenu(RenPyShowMenuStatement menu, int indentLevel)
    {
        var sb = new StringBuilder();
        var indent = new string(' ', indentLevel * 4);
        var choiceIndent = new string(' ', (indentLevel + 1) * 4);
        var actionIndent = new string(' ', (indentLevel + 2) * 4);

        sb.AppendLine($"{indent}menu:");
        
        foreach (var choice in menu.Choices)
        {
            sb.AppendLine($"{choiceIndent}\"{choice.Text}\":");
            sb.AppendLine($"{actionIndent}jump {choice.TargetLabel}");
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Formats position value for Ren'Py.
    /// Since frontend always sends relative coordinates (0.0-1.0),
    /// we always format as float with 2 decimal places.
    /// Ren'Py interprets float values as relative positions (percentage of screen size).
    /// </summary>
    private string FormatPosition(double position)
    {
        return position.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
    }
}

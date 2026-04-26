using NoviVovi.Infrastructure.Exporters.RenPy.Core.Menu.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Models;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Script;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Services.Script;

public class RenPyStatementRendererTests
{
    private readonly RenPyStatementRenderer _renderer = new();

    [Fact]
    public void Render_SceneStatement_ReturnsCorrectSyntax()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 0.0, YPos: 0.0, Zoom: 1.0, XZoom: 1.0, YZoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPySceneStatement("bg_classroom", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        var expected = """
    scene bg_classroom:
        xpos 0.00
        ypos 0.00
        zoom 1.00
        xzoom 1.00
        yzoom 1.00
        zorder 0
""";
        Assert.Equal(expected.ReplaceLineEndings(), result.ReplaceLineEndings());
    }

    [Fact]
    public void Render_SceneStatement_WithCustomIndent_ReturnsCorrectIndentation()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 0.0, YPos: 0.0, Zoom: 1.0, XZoom: 1.0, YZoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPySceneStatement("bg_park", transform);

        // Act
        var result = _renderer.Render(statement, indentLevel: 2);

        // Assert
        var expected = """
        scene bg_park:
            xpos 0.00
            ypos 0.00
            zoom 1.00
            xzoom 1.00
            yzoom 1.00
            zorder 0
""";
        Assert.Equal(expected.ReplaceLineEndings(), result.ReplaceLineEndings());
    }

    [Fact]
    public void Render_ShowCharacterStatement_UsesInlineATL()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 0.5, YPos: 0.5, Zoom: 1.0, XZoom: 1.0, YZoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPyShowCharacterStatement("alice", "neutral", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        var expected = """
    show alice neutral:
        xpos 0.50
        ypos 0.50
        zoom 1.00
        xzoom 1.00
        yzoom 1.00
        zorder 0
""";
        Assert.Equal(expected.ReplaceLineEndings(), result.ReplaceLineEndings());
    }

    [Fact]
    public void Render_ShowCharacterStatement_WithRotation_IncludesRotateProperty()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 0.6, YPos: 0.3, Zoom: 1.5, XZoom: 1.0, YZoom: 1.0, Rotate: 45.0, ZOrder: 1);
        var statement = new RenPyShowCharacterStatement("bob", "surprised", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        var expected = """
    show bob surprised:
        xpos 0.60
        ypos 0.30
        zoom 1.50
        xzoom 1.00
        yzoom 1.00
        rotate 45.00
        zorder 1
""";
        Assert.Equal(expected.ReplaceLineEndings(), result.ReplaceLineEndings());
    }

    [Fact]
    public void Render_ShowCharacterStatement_WithZeroRotation_OmitsRotateProperty()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 0.25, YPos: 0.75, Zoom: 0.8, XZoom: 1.5, YZoom: 0.5, Rotate: 0.0, ZOrder: 2);
        var statement = new RenPyShowCharacterStatement("charlie", "happy", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.DoesNotContain("rotate", result);
        Assert.Contains("show charlie happy:", result);
        Assert.Contains("xpos 0.25", result);
        Assert.Contains("ypos 0.75", result);
        Assert.Contains("zoom 0.80", result);
        Assert.Contains("xzoom 1.50", result);
        Assert.Contains("yzoom 0.50", result);
        Assert.Contains("zorder 2", result);
    }

    [Fact]
    public void Render_ShowCharacterStatement_WithComplexTransform_FormatsCorrectly()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 0.9, YPos: 0.1, Zoom: 2.5, XZoom: 2.0, YZoom: 1.5, Rotate: 180.0, ZOrder: 10);
        var statement = new RenPyShowCharacterStatement("dave", "angry", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Contains("show dave angry:", result);
        Assert.Contains("xpos 0.90", result);
        Assert.Contains("ypos 0.10", result);
        Assert.Contains("zoom 2.50", result);
        Assert.Contains("xzoom 2.00", result);
        Assert.Contains("yzoom 1.50", result);
        Assert.Contains("rotate 180.00", result);
        Assert.Contains("zorder 10", result);
    }

    [Fact]
    public void Render_ShowCharacterStatement_WithCustomIndent_ReturnsCorrectIndentation()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 0.1, YPos: 0.2, Zoom: 1.0, XZoom: 1.0, YZoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPyShowCharacterStatement("eve", "neutral", transform);

        // Act
        var result = _renderer.Render(statement, indentLevel: 2);

        // Assert
        var expected = """
        show eve neutral:
            xpos 0.10
            ypos 0.20
            zoom 1.00
            xzoom 1.00
            yzoom 1.00
            zorder 0
""";
        Assert.Equal(expected.ReplaceLineEndings(), result.ReplaceLineEndings());
    }

    [Fact]
    public void Render_HideCharacterStatement_ReturnsCorrectSyntax()
    {
        // Arrange
        var statement = new RenPyHideCharacterStatement("eileen");

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    hide eileen", result);
    }

    [Fact]
    public void Render_ReplicaStatement_ReturnsCorrectSyntax()
    {
        // Arrange
        var statement = new RenPyReplicaStatement("char_eileen", "Hello, world!");

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    char_eileen \"Hello, world!\"", result);
    }

    [Fact]
    public void Render_ReplicaStatement_WithSpecialCharacters_ReturnsCorrectSyntax()
    {
        // Arrange
        var statement = new RenPyReplicaStatement("char_alice", "She said: \"Really?\"");

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    char_alice \"She said: \"Really?\"\"", result);
    }

    [Fact]
    public void Render_ReplicaStatement_WithCyrillicText_ReturnsCorrectSyntax()
    {
        // Arrange
        var statement = new RenPyReplicaStatement("char_natasha", "Привет, мир!");

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    char_natasha \"Привет, мир!\"", result);
    }

    [Fact]
    public void Render_JumpStatement_ReturnsCorrectSyntax()
    {
        // Arrange
        var statement = new RenPyJumpStatement("label_chapter2");

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    jump label_chapter2", result);
    }

    [Fact]
    public void Render_ReturnStatement_ReturnsCorrectSyntax()
    {
        // Arrange
        var statement = new RenPyReturnStatement();

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    return", result);
    }

    [Fact]
    public void Render_ShowMenuStatement_WithSingleChoice_ReturnsCorrectSyntax()
    {
        // Arrange
        var choices = new List<RenPyChoice>
        {
            new("Continue", "label_continue")
        };
        var statement = new RenPyShowMenuStatement(choices);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        var expected = """
    menu:
        "Continue":
            jump label_continue
""";
        Assert.Equal(expected.ReplaceLineEndings(), result.ReplaceLineEndings());
    }

    [Fact]
    public void Render_ShowMenuStatement_WithMultipleChoices_ReturnsCorrectSyntax()
    {
        // Arrange
        var choices = new List<RenPyChoice>
        {
            new("Go left", "label_left"),
            new("Go right", "label_right"),
            new("Stay here", "label_stay")
        };
        var statement = new RenPyShowMenuStatement(choices);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        var expected = """
    menu:
        "Go left":
            jump label_left
        "Go right":
            jump label_right
        "Stay here":
            jump label_stay
""";
        Assert.Equal(expected.ReplaceLineEndings(), result.ReplaceLineEndings());
    }

    [Fact]
    public void Render_ShowMenuStatement_WithCyrillicChoices_ReturnsCorrectSyntax()
    {
        // Arrange
        var choices = new List<RenPyChoice>
        {
            new("Пойти налево", "label_left"),
            new("Пойти направо", "label_right")
        };
        var statement = new RenPyShowMenuStatement(choices);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        var expected = """
    menu:
        "Пойти налево":
            jump label_left
        "Пойти направо":
            jump label_right
""";
        Assert.Equal(expected.ReplaceLineEndings(), result.ReplaceLineEndings());
    }

    [Fact]
    public void Render_ShowMenuStatement_WithCustomIndent_ReturnsCorrectIndentation()
    {
        // Arrange
        var choices = new List<RenPyChoice>
        {
            new("Yes", "label_yes"),
            new("No", "label_no")
        };
        var statement = new RenPyShowMenuStatement(choices);

        // Act
        var result = _renderer.Render(statement, indentLevel: 2);

        // Assert
        var expected = """
        menu:
            "Yes":
                jump label_yes
            "No":
                jump label_no
""";
        Assert.Equal(expected.ReplaceLineEndings(), result.ReplaceLineEndings());
    }

    [Fact]
    public void Render_IndentLevel0_ReturnsNoIndentation()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 0.0, YPos: 0.0, Zoom: 1.0, XZoom: 1.0, YZoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPySceneStatement("bg_test", transform);

        // Act
        var result = _renderer.Render(statement, indentLevel: 0);

        // Assert
        Assert.StartsWith("scene bg_test:", result);
        Assert.Contains("xpos 0.00", result);
        Assert.Contains("xzoom 1.00", result);
    }

    [Fact]
    public void Render_IndentLevel3_ReturnsCorrectIndentation()
    {
        // Arrange
        var statement = new RenPyJumpStatement("label_test");

        // Act
        var result = _renderer.Render(statement, indentLevel: 3);

        // Assert
        Assert.Equal("            jump label_test", result);
    }
}

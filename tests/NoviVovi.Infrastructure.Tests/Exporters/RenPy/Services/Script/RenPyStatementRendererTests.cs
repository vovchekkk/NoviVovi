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
        var statement = new RenPySceneStatement("bg_classroom");

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    scene bg_classroom", result);
    }

    [Fact]
    public void Render_SceneStatement_WithCustomIndent_ReturnsCorrectIndentation()
    {
        // Arrange
        var statement = new RenPySceneStatement("bg_park");

        // Act
        var result = _renderer.Render(statement, indentLevel: 2);

        // Assert
        Assert.Equal("        scene bg_park", result);
    }

    [Fact]
    public void Render_ShowCharacterStatement_WithoutTransform_ReturnsCorrectSyntax()
    {
        // Arrange
        var statement = new RenPyShowCharacterStatement("eileen", "happy");

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    show eileen happy", result);
    }

    [Fact]
    public void Render_ShowCharacterStatement_WithLeftTransform_ReturnsCorrectSyntax()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 200, YPos: 0, Zoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPyShowCharacterStatement("alice", "neutral", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    show alice neutral at left", result);
    }

    [Fact]
    public void Render_ShowCharacterStatement_WithCenterTransform_ReturnsCorrectSyntax()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 640, YPos: 0, Zoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPyShowCharacterStatement("bob", "surprised", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    show bob surprised at center", result);
    }

    [Fact]
    public void Render_ShowCharacterStatement_WithRightTransform_ReturnsCorrectSyntax()
    {
        // Arrange
        var transform = new RenPyTransform(XPos: 1000, YPos: 0, Zoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPyShowCharacterStatement("charlie", "angry", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Equal("    show charlie angry at right", result);
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
        var statement = new RenPySceneStatement("bg_test");

        // Act
        var result = _renderer.Render(statement, indentLevel: 0);

        // Assert
        Assert.Equal("scene bg_test", result);
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

    [Fact]
    public void Render_TransformBoundaryLeft_ReturnsLeftPosition()
    {
        // Arrange - XPos = 399 (just below 400 threshold)
        var transform = new RenPyTransform(XPos: 399, YPos: 0, Zoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPyShowCharacterStatement("char", "state", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Contains("at left", result);
    }

    [Fact]
    public void Render_TransformBoundaryRight_ReturnsRightPosition()
    {
        // Arrange - XPos = 881 (just above 880 threshold)
        var transform = new RenPyTransform(XPos: 881, YPos: 0, Zoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPyShowCharacterStatement("char", "state", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Contains("at right", result);
    }

    [Fact]
    public void Render_TransformBoundaryCenter_ReturnsCenterPosition()
    {
        // Arrange - XPos = 400 (at lower center boundary)
        var transform = new RenPyTransform(XPos: 400, YPos: 0, Zoom: 1.0, Rotate: 0.0, ZOrder: 0);
        var statement = new RenPyShowCharacterStatement("char", "state", transform);

        // Act
        var result = _renderer.Render(statement);

        // Assert
        Assert.Contains("at center", result);
    }
}

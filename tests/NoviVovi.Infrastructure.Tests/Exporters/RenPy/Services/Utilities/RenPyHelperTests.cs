using NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;
using Xunit;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Services.Utilities;

/// <summary>
/// Tests for RenPyHelper utility methods.
/// </summary>
public class RenPyHelperTests
{
    [Fact]
    public void EscapeString_SimpleText_ReturnsUnchanged()
    {
        // Arrange
        var input = "Hello World";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("Hello World", result);
    }

    [Fact]
    public void EscapeString_TextWithQuotes_EscapesQuotes()
    {
        // Arrange
        var input = "She said \"Hello\"";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("She said \\\"Hello\\\"", result);
    }

    [Fact]
    public void EscapeString_TextWithBackslash_EscapesBackslash()
    {
        // Arrange
        var input = "Path\\to\\file";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("Path\\\\to\\\\file", result);
    }

    [Fact]
    public void EscapeString_TextWithNewline_EscapesNewline()
    {
        // Arrange
        var input = "Line1\nLine2";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("Line1\\nLine2", result);
    }

    [Fact]
    public void EscapeString_TextWithCarriageReturn_EscapesCarriageReturn()
    {
        // Arrange
        var input = "Line1\rLine2";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("Line1\\rLine2", result);
    }

    [Fact]
    public void EscapeString_TextWithAllSpecialChars_EscapesAll()
    {
        // Arrange
        var input = "Text with \"quotes\", \\backslash, \nnewline, and \rcarriage return";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("Text with \\\"quotes\\\", \\\\backslash, \\nnewline, and \\rcarriage return", result);
    }

    [Fact]
    public void EscapeString_EmptyString_ReturnsEmpty()
    {
        // Arrange
        var input = "";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void EscapeString_CyrillicText_PreservesCyrillic()
    {
        // Arrange
        var input = "Привет \"мир\"";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("Привет \\\"мир\\\"", result);
    }

    [Fact]
    public void EscapeString_MultipleBackslashes_EscapesEach()
    {
        // Arrange
        var input = "\\\\\\";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("\\\\\\\\\\\\", result);
    }

    [Fact]
    public void EscapeString_BackslashBeforeQuote_EscapesBoth()
    {
        // Arrange
        var input = "\\\"test\\\"";

        // Act
        var result = RenPyHelper.EscapeString(input);

        // Assert
        Assert.Equal("\\\\\\\"test\\\\\\\"", result);
    }
}

using NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;
using Xunit;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Services.Utilities;

/// <summary>
/// Tests for RenPyIdentifierGenerator.
/// </summary>
public class RenPyIdentifierGeneratorTests
{
    private readonly RenPyIdentifierGenerator _generator;

    public RenPyIdentifierGeneratorTests()
    {
        _generator = new RenPyIdentifierGenerator();
    }

    [Fact]
    public void GenerateForLabel_ValidGuid_ReturnsCorrectFormat()
    {
        // Arrange
        var labelId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

        // Act
        var result = _generator.GenerateForLabel(labelId);

        // Assert
        Assert.Equal("label_a1b2c3d4e5f67890abcdef1234567890", result);
    }

    [Fact]
    public void GenerateForLabel_SameGuidTwice_ReturnsCachedValue()
    {
        // Arrange
        var labelId = Guid.NewGuid();

        // Act
        var result1 = _generator.GenerateForLabel(labelId);
        var result2 = _generator.GenerateForLabel(labelId);

        // Assert
        Assert.Equal(result1, result2);
        Assert.Same(result1, result2); // Should be same instance from cache
    }

    [Fact]
    public void GenerateForCharacter_ValidGuid_ReturnsCorrectFormat()
    {
        // Arrange
        var characterId = Guid.Parse("12345678-90ab-cdef-1234-567890abcdef");

        // Act
        var result = _generator.GenerateForCharacter(characterId);

        // Assert
        Assert.Equal("char_1234567890abcdef1234567890abcdef", result);
    }

    [Fact]
    public void GenerateForCharacter_SameGuidTwice_ReturnsCachedValue()
    {
        // Arrange
        var characterId = Guid.NewGuid();

        // Act
        var result1 = _generator.GenerateForCharacter(characterId);
        var result2 = _generator.GenerateForCharacter(characterId);

        // Assert
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void GenerateForCharacterState_ValidGuid_ReturnsCorrectFormat()
    {
        // Arrange
        var stateId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");

        // Act
        var result = _generator.GenerateForCharacterState(stateId);

        // Assert
        Assert.Equal("state_aaaaaaaabbbbccccddddeeeeeeeeeeee", result);
    }

    [Fact]
    public void GenerateForCharacterState_SameGuidTwice_ReturnsCachedValue()
    {
        // Arrange
        var stateId = Guid.NewGuid();

        // Act
        var result1 = _generator.GenerateForCharacterState(stateId);
        var result2 = _generator.GenerateForCharacterState(stateId);

        // Assert
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void GenerateForLabel_DifferentGuids_ReturnsDifferentIdentifiers()
    {
        // Arrange
        var labelId1 = Guid.NewGuid();
        var labelId2 = Guid.NewGuid();

        // Act
        var result1 = _generator.GenerateForLabel(labelId1);
        var result2 = _generator.GenerateForLabel(labelId2);

        // Assert
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void GenerateForCharacter_NoHyphensInOutput()
    {
        // Arrange
        var characterId = Guid.NewGuid();

        // Act
        var result = _generator.GenerateForCharacter(characterId);

        // Assert
        Assert.DoesNotContain("-", result);
    }

    [Fact]
    public void GenerateForLabel_OutputIsPythonCompatible()
    {
        // Arrange
        var labelId = Guid.NewGuid();

        // Act
        var result = _generator.GenerateForLabel(labelId);

        // Assert
        Assert.Matches(@"^[a-z_][a-z0-9_]*$", result); // Valid Python identifier
    }

    [Fact]
    public void Cache_WorksAcrossDifferentMethods()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var labelResult = _generator.GenerateForLabel(id);
        var charResult = _generator.GenerateForCharacter(id);

        // Assert - same ID should return same cached value regardless of method
        Assert.Equal(labelResult, charResult);
    }
}

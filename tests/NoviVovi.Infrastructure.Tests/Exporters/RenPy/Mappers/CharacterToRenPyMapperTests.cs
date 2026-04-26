using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Characters.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Mappers;

public class CharacterToRenPyMapperTests
{
    private readonly RenPyIdentifierGenerator _idGenerator;
    private readonly CharacterToRenPyMapper _mapper;

    public CharacterToRenPyMapperTests()
    {
        _idGenerator = new RenPyIdentifierGenerator();
        _mapper = new CharacterToRenPyMapper(_idGenerator);
    }

    [Fact]
    public void Map_ValidCharacter_ReturnsRenPyCharacter()
    {
        // Arrange
        var character = Character.Create("Alice", Guid.NewGuid(), Color.FromHex("#FF5733"), "Main protagonist");

        // Act
        var result = _mapper.Map(character);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.DisplayName);
        Assert.Equal("#FF5733", result.Color);
        Assert.StartsWith("char_", result.VariableName);
    }

    [Fact]
    public void Map_CharacterWithShortHexColor_ExpandsToFullHex()
    {
        // Arrange
        var character = Character.Create("Bob", Guid.NewGuid(), Color.FromHex("#F73"), "Side character");

        // Act
        var result = _mapper.Map(character);

        // Assert
        Assert.Equal("#FF7733", result.Color);
    }

    [Fact]
    public void Map_CharacterWithoutHashInColor_AddsHash()
    {
        // Arrange
        var character = Character.Create("Charlie", Guid.NewGuid(), Color.FromHex("00FF00"), null);

        // Act
        var result = _mapper.Map(character);

        // Assert
        Assert.StartsWith("#", result.Color);
        Assert.Equal("#00FF00", result.Color);
    }

    [Fact]
    public void Map_CharacterWithNullDescription_MapsCorrectly()
    {
        // Arrange
        var character = Character.Create("Charlie", Guid.NewGuid(), Color.FromHex("00FF00"), null);

        // Act
        var result = _mapper.Map(character);

        // Assert
        Assert.StartsWith("#", result.Color);
        Assert.Equal("#00FF00", result.Color);
    }

    [Fact]
    public void Map_SameCharacterTwice_ReturnsSameVariableName()
    {
        // Arrange
        var character = Character.Create("Dave", Guid.NewGuid(), Color.FromHex("#123456"), null);

        // Act
        var result1 = _mapper.Map(character);
        var result2 = _mapper.Map(character);

        // Assert
        Assert.Equal(result1.VariableName, result2.VariableName);
    }

    [Fact]
    public void Map_DifferentCharacters_ReturnsDifferentVariableNames()
    {
        // Arrange
        var character1 = Character.Create("Eve", Guid.NewGuid(), Color.FromHex("#AAAAAA"), null);
        var character2 = Character.Create("Frank", Guid.NewGuid(), Color.FromHex("#BBBBBB"), null);

        // Act
        var result1 = _mapper.Map(character1);
        var result2 = _mapper.Map(character2);

        // Assert
        Assert.NotEqual(result1.VariableName, result2.VariableName);
    }

    [Fact]
    public void Map_CharacterWithSpecialCharactersInName_PreservesName()
    {
        // Arrange
        var character = Character.Create("Мария О'Коннор", Guid.NewGuid(), Color.FromHex("#FFFFFF"), null);

        // Act
        var result = _mapper.Map(character);

        // Assert
        Assert.Equal("Мария О'Коннор", result.DisplayName);
    }

    [Fact]
    public void Map_CharacterWithEmptyDescription_MapsSuccessfully()
    {
        // Arrange
        var character = Character.Create("Grace", Guid.NewGuid(), Color.FromHex("#CCCCCC"), null);

        // Act
        var result = _mapper.Map(character);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Grace", result.DisplayName);
    }

    [Fact]
    public void Map_CharacterWithLongName_MapsSuccessfully()
    {
        // Arrange
        var longName = new string('A', 100);
        var character = Character.Create(longName, Guid.NewGuid(), Color.FromHex("#DDDDDD"), null);

        // Act
        var result = _mapper.Map(character);

        // Assert
        Assert.Equal(longName, result.DisplayName);
    }

    [Fact]
    public void Map_VariableName_IsValidPythonIdentifier()
    {
        // Arrange
        var character = Character.Create("Test", Guid.NewGuid(), Color.FromHex("#EEEEEE"), null);

        // Act
        var result = _mapper.Map(character);

        // Assert
        Assert.Matches(@"^char_[a-f0-9]{32}$", result.VariableName);
    }

    [Fact]
    public void Map_ColorValue_IsUpperCase()
    {
        // Arrange
        var character = Character.Create("Test", Guid.NewGuid(), Color.FromHex("#abcdef"), null);

        // Act
        var result = _mapper.Map(character);

        // Assert
        Assert.Equal("#ABCDEF", result.Color);
    }
}




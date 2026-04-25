using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Characters.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Labels.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Novels.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Mappers;

public class NovelToRenPyMapperTests
{
    private readonly RenPyIdentifierGenerator _idGenerator;
    private readonly CharacterToRenPyMapper _characterMapper;
    private readonly TransformToRenPyMapper _transformMapper;
    private readonly StepToRenPyMapper _stepMapper;
    private readonly LabelToRenPyMapper _labelMapper;
    private readonly NovelToRenPyMapper _mapper;

    public NovelToRenPyMapperTests()
    {
        _idGenerator = new RenPyIdentifierGenerator();
        _characterMapper = new CharacterToRenPyMapper(_idGenerator);
        _transformMapper = new TransformToRenPyMapper();
        _stepMapper = new StepToRenPyMapper(_idGenerator, _transformMapper);
        _labelMapper = new LabelToRenPyMapper(_idGenerator, _stepMapper);
        _mapper = new NovelToRenPyMapper(_idGenerator, _characterMapper, _labelMapper);
    }

    [Fact]
    public void Map_ValidNovel_ReturnsRenPyNovel()
    {
        // Arrange
        var novel = Novel.Create("My Visual Novel", "start");

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("My Visual Novel", result.Title);
        Assert.NotNull(result.Characters);
        Assert.NotNull(result.Labels);
        Assert.StartsWith("label_", result.StartLabelId);
    }

    [Fact]
    public void Map_NovelWithoutCharacters_ReturnsEmptyCharactersList()
    {
        // Arrange
        var novel = Novel.Create("Empty Novel", "start");

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.Empty(result.Characters);
    }

    [Fact]
    public void Map_NovelWithCharacters_MapsAllCharacters()
    {
        // Arrange
        var novel = Novel.Create("Novel with Characters", "start");
        var char1 = Character.Create("Alice", Color.FromHex("#FF5733"), null);
        var char2 = Character.Create("Bob", Color.FromHex("#00FF00"), null);
        novel.AddCharacter(char1);
        novel.AddCharacter(char2);

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.Equal(2, result.Characters.Count);
        Assert.Contains(result.Characters, c => c.DisplayName == "Alice");
        Assert.Contains(result.Characters, c => c.DisplayName == "Bob");
    }

    [Fact]
    public void Map_NovelWithLabels_MapsAllLabels()
    {
        // Arrange
        var novel = Novel.Create("Novel with Labels", "start");
        var label1 = Label.Create("chapter1", novel.Id);
        var label2 = Label.Create("chapter2", novel.Id);
        novel.AddLabel(label1);
        novel.AddLabel(label2);

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.Equal(3, result.Labels.Count); // start + 2 additional
    }

    [Fact]
    public void Map_NovelWithStartLabel_MapsStartLabelIdCorrectly()
    {
        // Arrange
        var novel = Novel.Create("Test Novel", "beginning");

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.NotNull(result.StartLabelId);
        Assert.StartsWith("label_", result.StartLabelId);
    }

    [Fact]
    public void Map_NovelWithCustomStartLabel_UsesCustomStartLabel()
    {
        // Arrange
        var novel = Novel.Create("Custom Start Novel", "start");
        var customStart = Label.Create("custom_start", novel.Id);
        novel.AddLabel(customStart);
        novel.SetStartLabel(customStart);

        // Act
        var result = _mapper.Map(novel);

        // Assert
        var customStartId = _idGenerator.GenerateForLabel(customStart.Id);
        Assert.Equal(customStartId, result.StartLabelId);
    }

    [Fact]
    public void Map_NovelWithComplexStructure_MapsEverything()
    {
        // Arrange
        var novel = Novel.Create("Complex Novel", "start");
        
        // Add characters
        var character = Character.Create("Protagonist", Color.FromHex("#FFFFFF"), "Main character");
        novel.AddCharacter(character);
        
        // Add labels with steps
        var chapter1 = Label.Create("chapter1", novel.Id);
        var chapter2 = Label.Create("chapter2", novel.Id);
        
        var jumpStep = JumpStep.Create(chapter2);
        chapter1.AddStep(jumpStep);
        
        novel.AddLabel(chapter1);
        novel.AddLabel(chapter2);

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.Equal("Complex Novel", result.Title);
        Assert.Single(result.Characters);
        Assert.Equal(3, result.Labels.Count); // start + chapter1 + chapter2
        Assert.NotEmpty(result.StartLabelId);
    }

    [Fact]
    public void Map_NovelWithSpecialCharactersInTitle_PreservesTitle()
    {
        // Arrange
        var novel = Novel.Create("Моя Визуальная Новелла: Часть 1", "start");

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.Equal("Моя Визуальная Новелла: Часть 1", result.Title);
    }

    [Fact]
    public void Map_NovelWithLongTitle_MapsSuccessfully()
    {
        // Arrange
        var longTitle = new string('A', 500);
        var novel = Novel.Create(longTitle, "start");

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.Equal(longTitle, result.Title);
    }

    [Fact]
    public void Map_SameNovelTwice_ReturnsConsistentIdentifiers()
    {
        // Arrange
        var novel = Novel.Create("Consistent Novel", "start");
        var character = Character.Create("Test", Color.FromHex("#123456"), null);
        novel.AddCharacter(character);

        // Act
        var result1 = _mapper.Map(novel);
        var result2 = _mapper.Map(novel);

        // Assert
        Assert.Equal(result1.StartLabelId, result2.StartLabelId);
        Assert.Equal(result1.Characters[0].VariableName, result2.Characters[0].VariableName);
    }

    [Fact]
    public void Map_NovelWithMultipleCharactersAndLabels_PreservesOrder()
    {
        // Arrange
        var novel = Novel.Create("Ordered Novel", "start");
        
        var char1 = Character.Create("First", Color.FromHex("#111111"), null);
        var char2 = Character.Create("Second", Color.FromHex("#222222"), null);
        var char3 = Character.Create("Third", Color.FromHex("#333333"), null);
        
        novel.AddCharacter(char1);
        novel.AddCharacter(char2);
        novel.AddCharacter(char3);
        
        var label1 = Label.Create("label1", novel.Id);
        var label2 = Label.Create("label2", novel.Id);
        
        novel.AddLabel(label1);
        novel.AddLabel(label2);

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.Equal("First", result.Characters[0].DisplayName);
        Assert.Equal("Second", result.Characters[1].DisplayName);
        Assert.Equal("Third", result.Characters[2].DisplayName);
    }

    [Fact]
    public void Map_NovelWithCharactersWithStates_MapsSuccessfully()
    {
        // Arrange
        var novel = Novel.Create("Novel with States", "start");
        var character = Character.Create("Hero", Color.FromHex("#FF0000"), null);
        
        var image = Image.CreatePending("sprite1", "/path/to/sprite.png", "png", ImageType.Character, new Size(500, 800));
        var transform = Transform.Create(new Position(0, 0), new Size(500, 800));
        var state = CharacterState.Create("happy", image, transform);
        
        character.AddCharacterState(state);
        novel.AddCharacter(character);

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.Single(result.Characters);
        Assert.Equal("Hero", result.Characters[0].DisplayName);
    }

    [Fact]
    public void Map_MinimalNovel_MapsSuccessfully()
    {
        // Arrange
        var novel = Novel.Create("Minimal", "start");

        // Act
        var result = _mapper.Map(novel);

        // Assert
        Assert.Equal("Minimal", result.Title);
        Assert.Empty(result.Characters);
        Assert.Single(result.Labels); // Only start label
        Assert.NotEmpty(result.StartLabelId);
    }

    [Fact]
    public void Map_NovelStartLabelId_MatchesStartLabelIdentifier()
    {
        // Arrange
        var novel = Novel.Create("Test", "start");

        // Act
        var result = _mapper.Map(novel);

        // Assert
        var startLabel = result.Labels.First(l => l.Identifier == result.StartLabelId);
        Assert.NotNull(startLabel);
    }
}

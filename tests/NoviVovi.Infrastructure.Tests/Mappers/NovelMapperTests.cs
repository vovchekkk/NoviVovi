using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests.Mappers;

public class NovelMapperTests
{
    private readonly TransformMapper _transformMapper = new();
    private readonly ImageMapper _imageMapper;
    private readonly CharacterMapper _characterMapper;
    private readonly ReplicaMapper _replicaMapper;
    private readonly Lazy<LabelMapper> _labelMapper;
    private readonly Lazy<MenuMapper> _menuMapper;
    private readonly Lazy<StepMapper> _stepMapper;
    private readonly LabelMapper _labelMapperInstance;
    private readonly NovelMapper _mapper;

    public NovelMapperTests()
    {
        _imageMapper = new ImageMapper(_transformMapper);
        _characterMapper = new CharacterMapper(_imageMapper, _transformMapper);
        _replicaMapper = new ReplicaMapper(_characterMapper);
        
        _labelMapper = new Lazy<LabelMapper>(() => _labelMapperInstance);
        _menuMapper = new Lazy<MenuMapper>(() => new MenuMapper(_labelMapper));
        _stepMapper = new Lazy<StepMapper>(() => new StepMapper(
            _labelMapper,
            _imageMapper,
            _characterMapper,
            _menuMapper,
            _replicaMapper
        ));
        
        _labelMapperInstance = new LabelMapper(_stepMapper);
        _mapper = new NovelMapper(_characterMapper, _labelMapperInstance);
    }

    [Fact]
    public void ToDomain_ValidNovelDbO_WithoutCharactersAndLabels_ReturnsNovel()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var startLabelId = Guid.NewGuid();
        
        var dbo = new NovelDbO
        {
            Id = novelId,
            Title = "Test Novel",
            StartLabelId = startLabelId,
            IsPublic = true,
            StartLabel = new LabelDbO
            {
                Id = startLabelId,
                LabelName = "start",
                NovelId = novelId,
                Steps = []
            },
            Labels = [],
            Characters = []
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(novelId, result.Id);
        Assert.Equal("Test Novel", result.Title);
        Assert.NotNull(result.StartLabel);
        Assert.Equal(startLabelId, result.StartLabel.Id);
        Assert.Single(result.Labels); // StartLabel is automatically added to Labels
        Assert.Empty(result.Characters);
    }

    [Fact]
    public void ToDomain_NovelDbO_WithoutStartLabel_ThrowsArgumentException()
    {
        // Arrange
        var dbo = new NovelDbO
        {
            Id = Guid.NewGuid(),
            Title = "Invalid Novel",
            StartLabelId = null,
            IsPublic = true,
            StartLabel = null,
            Labels = [],
            Characters = []
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _mapper.ToDomain(dbo));
        Assert.Equal("Novel startLabel is null", exception.Message);
    }

    [Fact]
    public void ToDomain_NovelDbO_WithCharacters_ReturnsNovelWithCharacters()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var startLabelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        
        var dbo = new NovelDbO
        {
            Id = novelId,
            Title = "Novel with Characters",
            StartLabelId = startLabelId,
            IsPublic = true,
            StartLabel = new LabelDbO
            {
                Id = startLabelId,
                LabelName = "start",
                NovelId = novelId,
                Steps = []
            },
            Labels = [],
            Characters =
            [
                new CharacterDbO
                {
                    Id = characterId,
                    Name = "Alice",
                    NovelId = novelId,
                    NameColor = "FF5733",
                    Description = "Main character",
                    States = []
                }
            ]
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Characters);
        var character = result.Characters.First();
        Assert.Equal(characterId, character.Id);
        Assert.Equal("Alice", character.Name);
    }

    [Fact]
    public void ToDomain_NovelDbO_WithMultipleLabels_ReturnsNovelWithLabels()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var startLabelId = Guid.NewGuid();
        var label2Id = Guid.NewGuid();
        var label3Id = Guid.NewGuid();
        
        var dbo = new NovelDbO
        {
            Id = novelId,
            Title = "Novel with Labels",
            StartLabelId = startLabelId,
            IsPublic = true,
            StartLabel = new LabelDbO
            {
                Id = startLabelId,
                LabelName = "start",
                NovelId = novelId,
                Steps = []
            },
            Labels =
            [
                new LabelDbO
                {
                    Id = startLabelId,
                    LabelName = "start",
                    NovelId = novelId,
                    Steps = []
                },
                new LabelDbO
                {
                    Id = label2Id,
                    LabelName = "scene1",
                    NovelId = novelId,
                    Steps = []
                },
                new LabelDbO
                {
                    Id = label3Id,
                    LabelName = "scene2",
                    NovelId = novelId,
                    Steps = []
                }
            ],
            Characters = []
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        // StartLabel автоматически добавляется в Labels при создании Novel
        Assert.Equal(3, result.Labels.Count);
        Assert.Contains(result.Labels, l => l.Id == startLabelId);
        Assert.Contains(result.Labels, l => l.Id == label2Id);
        Assert.Contains(result.Labels, l => l.Id == label3Id);
    }

    [Fact]
    public void ToDbO_ValidNovel_ReturnsNovelDbO()
    {
        // Arrange
        var startLabel = new Label(Guid.NewGuid(), "start", Guid.NewGuid());
        var novel = new Novel(Guid.NewGuid(), "My Novel", startLabel);

        // Act
        var result = _mapper.ToDbO(novel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(novel.Id, result.Id);
        Assert.Equal("My Novel", result.Title);
        Assert.Equal(startLabel.Id, result.StartLabelId);
        Assert.NotNull(result.StartLabel);
        Assert.True(result.IsPublic);
        Assert.Empty(result.Characters);
        Assert.Single(result.Labels); // Only start label
    }

    [Fact]
    public void ToDbO_Novel_WithCharacters_ReturnsNovelDbOWithCharacters()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var startLabel = new Label(Guid.NewGuid(), "start", novelId);
        var novel = new Novel(Guid.NewGuid(), "Novel with Chars", startLabel);
        
        var character1 = new Character(Guid.NewGuid(), "Bob", novelId, Color.FromHex("00FF00"), null);
        var character2 = new Character(Guid.NewGuid(), "Charlie", novelId, Color.FromHex("0000FF"), "Side character");
        
        novel.AddCharacter(character1);
        novel.AddCharacter(character2);

        // Act
        var result = _mapper.ToDbO(novel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Characters.Count);
        Assert.Contains(result.Characters, c => c.Id == character1.Id);
        Assert.Contains(result.Characters, c => c.Id == character2.Id);
    }

    [Fact]
    public void ToDbO_Novel_WithMultipleLabels_ReturnsNovelDbOWithAllLabels()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var startLabel = new Label(Guid.NewGuid(), "start", novelId);
        var novel = new Novel(Guid.NewGuid(), "Multi Label Novel", startLabel);
        
        var label2 = new Label(Guid.NewGuid(), "scene1", novelId);
        var label3 = new Label(Guid.NewGuid(), "scene2", novelId);
        
        novel.AddLabel(label2);
        novel.AddLabel(label3);

        // Act
        var result = _mapper.ToDbO(novel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Labels.Count);
        Assert.Contains(result.Labels, l => l!.Id == startLabel.Id);
        Assert.Contains(result.Labels, l => l!.Id == label2.Id);
        Assert.Contains(result.Labels, l => l!.Id == label3.Id);
    }

    [Fact]
    public void RoundTrip_Novel_WithoutCharactersAndLabels_PreservesAllValues()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var startLabel = new Label(Guid.NewGuid(), "start", novelId);
        var original = new Novel(Guid.NewGuid(), "Test Novel", startLabel);

        // Act
        var dbo = _mapper.ToDbO(original);
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Title, result.Title);
        Assert.Equal(original.StartLabel.Id, result.StartLabel.Id);
        Assert.Equal(original.StartLabel.Name, result.StartLabel.Name);
    }

    [Fact]
    public void RoundTrip_Novel_WithCharactersAndLabels_PreservesAllValues()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var startLabel = new Label(Guid.NewGuid(), "start", novelId);
        var original = new Novel(Guid.NewGuid(), "Complex Novel", startLabel);
        
        var character = new Character(Guid.NewGuid(), "Diana", novelId, Color.FromHex("FF00FF"), "Protagonist");
        original.AddCharacter(character);
        
        var label2 = new Label(Guid.NewGuid(), "chapter1", novelId);
        original.AddLabel(label2);

        // Act
        var dbo = _mapper.ToDbO(original);
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Title, result.Title);
        Assert.Single(result.Characters);
        Assert.Equal(character.Id, result.Characters.First().Id);
        Assert.Equal(2, result.Labels.Count); // StartLabel + label2
        Assert.Contains(result.Labels, l => l.Id == label2.Id);
    }

    [Fact]
    public void ToDbO_Novel_WithNullStartLabel_ReturnsNovelDbOWithNullStartLabel()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var startLabel = new Label(Guid.NewGuid(), "start", novelId);
        var novel = new Novel(Guid.NewGuid(), "Test", startLabel);
        
        // Use reflection to set StartLabel to null (for testing edge case)
        var startLabelField = typeof(Novel).GetField("<StartLabel>k__BackingField", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        startLabelField?.SetValue(novel, null);

        // Act
        var result = _mapper.ToDbO(novel);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.StartLabelId);
        Assert.Null(result.StartLabel);
    }

    [Fact]
    public void ToDomain_NovelDbO_WithLongTitle_PreservesTitle()
    {
        // Arrange
        var longTitle = new string('A', 500);
        var startLabelId = Guid.NewGuid();
        var novelId = Guid.NewGuid();
        
        var dbo = new NovelDbO
        {
            Id = novelId,
            Title = longTitle,
            StartLabelId = startLabelId,
            IsPublic = true,
            StartLabel = new LabelDbO
            {
                Id = startLabelId,
                LabelName = "start",
                NovelId = novelId,
                Steps = []
            },
            Labels = [],
            Characters = []
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(longTitle, result.Title);
        Assert.Equal(500, result.Title.Length);
    }

    [Fact]
    public void ToDbO_Novel_WithSpecialCharactersInTitle_PreservesTitle()
    {
        // Arrange
        var specialTitle = "Novel: \"The Adventure\" & 'The Journey' <Part 1>";
        var startLabel = new Label(Guid.NewGuid(), "start", Guid.NewGuid());
        var novel = new Novel(Guid.NewGuid(), specialTitle, startLabel);

        // Act
        var result = _mapper.ToDbO(novel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(specialTitle, result.Title);
    }
}

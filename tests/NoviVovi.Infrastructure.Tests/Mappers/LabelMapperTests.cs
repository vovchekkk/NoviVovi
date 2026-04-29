using NoviVovi.Domain.Characters;
using NoviVovi.Domain.Common;
using NoviVovi.Domain.Dialogue;
using NoviVovi.Domain.Images;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Scene;
using NoviVovi.Domain.Steps;
using NoviVovi.Domain.Transitions;
using NoviVovi.Infrastructure.DatabaseObjects.Characters;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests.Mappers;

public class LabelMapperTests
{
    private readonly TransformMapper _transformMapper = new();
    private readonly ImageMapper _imageMapper;
    private readonly CharacterMapper _characterMapper;
    private readonly ReplicaMapper _replicaMapper;
    private readonly Lazy<LabelMapper> _labelMapper;
    private readonly Lazy<MenuMapper> _menuMapper;
    private readonly Lazy<StepMapper> _stepMapper;
    private readonly LabelMapper _mapper;

    public LabelMapperTests()
    {
        _imageMapper = new ImageMapper(_transformMapper);
        _characterMapper = new CharacterMapper(_imageMapper, _transformMapper);
        _replicaMapper = new ReplicaMapper(_characterMapper);
        
        _labelMapper = new Lazy<LabelMapper>(() => _mapper);
        _menuMapper = new Lazy<MenuMapper>(() => new MenuMapper(_labelMapper));
        _stepMapper = new Lazy<StepMapper>(() => new StepMapper(
            _labelMapper,
            _imageMapper,
            _characterMapper,
            _menuMapper,
            _replicaMapper
        ));
        
        _mapper = new LabelMapper(_stepMapper);
    }

    [Fact]
    public void ToDomain_ValidLabelDbO_WithoutSteps_ReturnsLabel()
    {
        // Arrange
        var labelId = Guid.NewGuid();
        var novelId = Guid.NewGuid();
        
        var dbo = new LabelDbO
        {
            Id = labelId,
            LabelName = "start",
            NovelId = novelId,
            Steps = []
        };
        
        var ctx = new MappingContext();

        // Act
        var result = _mapper.ToDomain(dbo, ctx);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(labelId, result.Id);
        Assert.Equal("start", result.Name);
        Assert.Equal(novelId, result.NovelId);
        Assert.Empty(result.Steps);
    }

    [Fact]
    public void ToDomain_LabelDbO_WithSteps_ReturnsLabelWithSteps()
    {
        // Arrange
        var labelId = Guid.NewGuid();
        var novelId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var replicaId = Guid.NewGuid();
        
        var dbo = new LabelDbO
        {
            Id = labelId,
            LabelName = "scene1",
            NovelId = novelId,
            Steps =
            [
                new StepDbO
                {
                    Id = Guid.NewGuid(),
                    LabelId = labelId,
                    StepOrder = 0,
                    StepType = "show_replica",
                    ReplicaId = replicaId,
                    Replica = new ReplicaDbO
                    {
                        Id = replicaId,
                        SpeakerId = characterId,
                        Text = "Hello!",
                        Speaker = new CharacterDbO
                        {
                            Id = characterId,
                            Name = "Alice",
                            NovelId = novelId,
                            NameColor = "FF5733",
                            Description = null,
                            States = []
                        }
                    }
                }
            ]
        };
        
        var ctx = new MappingContext();

        // Act
        var result = _mapper.ToDomain(dbo, ctx);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Steps);
        Assert.IsType<ShowReplicaStep>(result.Steps[0]);
    }

    [Fact]
    public void ToDomain_SameLabelTwice_ReturnsCachedInstance()
    {
        // Arrange
        var labelId = Guid.NewGuid();
        var dbo = new LabelDbO
        {
            Id = labelId,
            LabelName = "test",
            NovelId = Guid.NewGuid(),
            Steps = []
        };
        
        var ctx = new MappingContext();

        // Act
        var result1 = _mapper.ToDomain(dbo, ctx);
        var result2 = _mapper.ToDomain(dbo, ctx);

        // Assert
        Assert.Same(result1, result2);
    }

    [Fact]
    public void ToDbO_ValidLabel_WithoutSteps_ReturnsLabelDbO()
    {
        // Arrange
        var label = new Label(Guid.NewGuid(), "intro", Guid.NewGuid());
        var ctx = new MappingContext();

        // Act
        var result = _mapper.ToDbO(label, ctx);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(label.Id, result.Id);
        Assert.Equal("intro", result.LabelName);
        Assert.Equal(label.NovelId, result.NovelId);
        Assert.Empty(result.Steps);
    }

    [Fact]
    public void ToDbO_Label_WithSteps_ReturnsLabelDbOWithSteps()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = new Label(Guid.NewGuid(), "scene", novelId);
        
        var character = new Character(
            Guid.NewGuid(),
            "Bob",
            novelId,
            Color.FromHex("00FF00"),
            null
        );
        
        var replica = new Replica(Guid.NewGuid(), character, "Test text");
        var step = new ShowReplicaStep(Guid.NewGuid(), replica, new NextStepTransition());
        
        label.AddStep(step);
        
        var ctx = new MappingContext();

        // Act
        var result = _mapper.ToDbO(label, ctx);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Steps);
        Assert.Equal(step.Id, result.Steps[0].Id);
        Assert.Equal("show_replica", result.Steps[0].StepType);
        Assert.Equal(0, result.Steps[0].StepOrder);
    }

    [Fact]
    public void ToDbO_Label_WithMultipleSteps_PreservesOrder()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = new Label(Guid.NewGuid(), "multi_step", novelId);
        
        var character = new Character(Guid.NewGuid(), "Charlie", novelId, Color.FromHex("0000FF"), null);
        
        var step1 = new ShowReplicaStep(
            Guid.NewGuid(),
            new Replica(Guid.NewGuid(), character, "First"),
            new NextStepTransition()
        );
        
        var step2 = new ShowReplicaStep(
            Guid.NewGuid(),
            new Replica(Guid.NewGuid(), character, "Second"),
            new NextStepTransition()
        );
        
        var step3 = new ShowReplicaStep(
            Guid.NewGuid(),
            new Replica(Guid.NewGuid(), character, "Third"),
            new NextStepTransition()
        );
        
        label.AddStep(step1);
        label.AddStep(step2);
        label.AddStep(step3);
        
        var ctx = new MappingContext();

        // Act
        var result = _mapper.ToDbO(label, ctx);

        // Assert
        Assert.Equal(3, result.Steps.Count);
        Assert.Equal(0, result.Steps[0].StepOrder);
        Assert.Equal(1, result.Steps[1].StepOrder);
        Assert.Equal(2, result.Steps[2].StepOrder);
        Assert.Equal(step1.Id, result.Steps[0].Id);
        Assert.Equal(step2.Id, result.Steps[1].Id);
        Assert.Equal(step3.Id, result.Steps[2].Id);
    }

    [Fact]
    public void ToDbO_NullLabel_ReturnsNull()
    {
        // Arrange
        Label? label = null;
        var ctx = new MappingContext();

        // Act
        var result = _mapper.ToDbO(label, ctx);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDbO_SameLabelTwice_ReturnsCachedInstance()
    {
        // Arrange
        var label = new Label(Guid.NewGuid(), "cached", Guid.NewGuid());
        var ctx = new MappingContext();

        // Act
        var result1 = _mapper.ToDbO(label, ctx);
        var result2 = _mapper.ToDbO(label, ctx);

        // Assert
        Assert.Same(result1, result2);
    }

    [Fact]
    public void RoundTrip_Label_WithoutSteps_PreservesAllValues()
    {
        // Arrange
        var original = new Label(Guid.NewGuid(), "test_label", Guid.NewGuid());
        var ctx = new MappingContext();

        // Act
        var dbo = _mapper.ToDbO(original, ctx);
        var result = _mapper.ToDomain(dbo!, new MappingContext());

        // Assert
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Name, result.Name);
        Assert.Equal(original.NovelId, result.NovelId);
        Assert.Empty(result.Steps);
    }

    [Fact]
    public void ToDomain_LabelDbO_WithShowBackgroundStep_ReturnsLabelWithStep()
    {
        // Arrange
        var labelId = Guid.NewGuid();
        var novelId = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var transformId = Guid.NewGuid();
        var backgroundId = Guid.NewGuid();
        
        var dbo = new LabelDbO
        {
            Id = labelId,
            LabelName = "bg_scene",
            NovelId = novelId,
            Steps =
            [
                new StepDbO
                {
                    Id = Guid.NewGuid(),
                    LabelId = labelId,
                    StepOrder = 0,
                    StepType = "show_background",
                    BackgroundId = backgroundId,
                    Background = new BackgroundDbO
                    {
                        Id = backgroundId,
                        Img = imageId,
                        TransformId = transformId,
                        Image = new ImageDbO
                        {
                            Id = imageId,
                            Name = "bg.png",
                            NovelId = novelId,
                            Url = "https://example.com/bg.png",
                            Format = "png",
                            ImgType = "background",
                            Width = 1920,
                            Height = 1080
                        },
                        Transform = new TransformDbO
                        {
                            Id = transformId,
                            Width = 1920,
                            Height = 1080,
                            Scale = 1m,
                            Rotation = 0m,
                            XPos = 0m,
                            YPos = 0m,
                            ZIndex = 0
                        }
                    }
                }
            ]
        };
        
        var ctx = new MappingContext();

        // Act
        var result = _mapper.ToDomain(dbo, ctx);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Steps);
        Assert.IsType<ShowBackgroundStep>(result.Steps[0]);
        var bgStep = (ShowBackgroundStep)result.Steps[0];
        Assert.Equal(backgroundId, bgStep.BackgroundObject.Id);
    }

    [Fact]
    public void ToDbO_Label_WithShowBackgroundStep_ReturnsCorrectDbO()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = new Label(Guid.NewGuid(), "bg_label", novelId);
        
        var image = new Image(
            Guid.NewGuid(),
            "background.png",
            novelId,
            "https://example.com/bg.png",
            "png",
            ImageType.Background,
            new Size(1920, 1080),
            ImageStatus.Active
        );
        
        var transform = new Transform(
            Guid.NewGuid(),
            new Position(0, 0),
            new Size(1920, 1080),
            1.0,
            0.0,
            0
        );
        
        var background = new BackgroundObject(Guid.NewGuid(), image, transform);
        var step = new ShowBackgroundStep(Guid.NewGuid(), background, new NextStepTransition());
        
        label.AddStep(step);
        
        var ctx = new MappingContext();

        // Act
        var result = _mapper.ToDbO(label, ctx);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Steps);
        Assert.Equal("show_background", result.Steps[0].StepType);
        Assert.Equal(background.Id, result.Steps[0].BackgroundId);
        Assert.NotNull(result.Steps[0].Background);
    }
}

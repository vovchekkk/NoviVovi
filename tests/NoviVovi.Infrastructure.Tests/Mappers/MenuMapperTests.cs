using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Menu;
using NoviVovi.Domain.Transitions;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests.Mappers;

public class MenuMapperTests
{
    private readonly TransformMapper _transformMapper = new();
    private readonly ImageMapper _imageMapper;
    private readonly CharacterMapper _characterMapper;
    private readonly ReplicaMapper _replicaMapper;
    private readonly Lazy<LabelMapper> _labelMapper;
    private readonly Lazy<StepMapper> _stepMapper;
    private readonly MenuMapper _mapper;

    public MenuMapperTests()
    {
        var ctx = new MappingContext();
        _imageMapper = new ImageMapper(_transformMapper);
        _characterMapper = new CharacterMapper(_imageMapper, _transformMapper, ctx);
        _replicaMapper = new ReplicaMapper(_characterMapper);
        
        _stepMapper = new Lazy<StepMapper>(() => new StepMapper(
            _labelMapper,
            _imageMapper,
            _characterMapper,
            new Lazy<MenuMapper>(() => _mapper),
            _replicaMapper,
            ctx
        ));
        
        _labelMapper = new Lazy<LabelMapper>(() => new LabelMapper(_stepMapper, ctx));
        _mapper = new MenuMapper(_labelMapper, ctx);
    }

    [Fact]
    public void ToDbO_ValidMenu_ReturnsMenuDbO()
    {
        // Arrange
        var menu = new Menu(Guid.NewGuid());
        var targetLabel = new Label(Guid.NewGuid(), "target_label", Guid.NewGuid());
        
        var choice1 = new Choice(
            Guid.NewGuid(),
            "Choice 1",
            new ChoiceTransition(targetLabel)
        );
        
        var choice2 = new Choice(
            Guid.NewGuid(),
            "Choice 2",
            new ChoiceTransition(targetLabel)
        );
        
        menu.AddChoice(choice1);
        menu.AddChoice(choice2);

        // Act
        var result = _mapper.ToDbO(menu);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(menu.Id, result.Id);
        Assert.Equal(2, result.Choices.Count);
        
        var choiceDbO1 = result.Choices[0];
        Assert.Equal(choice1.Id, choiceDbO1.Id);
        Assert.Equal("Choice 1", choiceDbO1.Text);
        Assert.Equal(menu.Id, choiceDbO1.MenuId);
        Assert.Equal(targetLabel.Id, choiceDbO1.NextLabelId);
        
        var choiceDbO2 = result.Choices[1];
        Assert.Equal(choice2.Id, choiceDbO2.Id);
        Assert.Equal("Choice 2", choiceDbO2.Text);
    }

    [Fact]
    public void ToDbO_EmptyMenu_ReturnsMenuDbOWithoutChoices()
    {
        // Arrange
        var menu = new Menu(Guid.NewGuid());

        // Act
        var result = _mapper.ToDbO(menu);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(menu.Id, result.Id);
        Assert.Empty(result.Choices);
    }

    [Fact]
    public void ToDomain_ValidMenuDbO_ReturnsMenu()
    {
        // Arrange
        var menuId = Guid.NewGuid();
        var targetLabelId = Guid.NewGuid();
        
        var dbo = new MenuDbO
        {
            Id = menuId,
            Choices =
            [
                new ChoiceDbO
                {
                    Id = Guid.NewGuid(),
                    Text = "Go left",
                    MenuId = menuId,
                    NextLabelId = targetLabelId,
                    NextLabel = new LabelDbO
                    {
                        Id = targetLabelId,
                        LabelName = "left_path",
                        NovelId = Guid.NewGuid(),
                        Steps = []
                    }
                },
                new ChoiceDbO
                {
                    Id = Guid.NewGuid(),
                    Text = "Go right",
                    MenuId = menuId,
                    NextLabelId = targetLabelId,
                    NextLabel = new LabelDbO
                    {
                        Id = targetLabelId,
                        LabelName = "right_path",
                        NovelId = Guid.NewGuid(),
                        Steps = []
                    }
                }
            ]
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(menuId, result.Id);
        Assert.Equal(2, result.Choices.Count);
        
        var choice1 = result.Choices[0];
        Assert.Equal("Go left", choice1.Text);
        Assert.NotNull(choice1.Transition);
        Assert.IsType<ChoiceTransition>(choice1.Transition);
        
        var choice2 = result.Choices[1];
        Assert.Equal("Go right", choice2.Text);
    }

    [Fact]
    public void ToDomain_MenuDbO_WithoutChoices_ReturnsEmptyMenu()
    {
        // Arrange
        var dbo = new MenuDbO
        {
            Id = Guid.NewGuid(),
            Choices = []
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Choices);
    }

    [Fact]
    public void RoundTrip_Menu_PreservesAllValues()
    {
        // Arrange
        var targetLabel = new Label(Guid.NewGuid(), "target", Guid.NewGuid());
        var original = new Menu(Guid.NewGuid());
        
        var choice = new Choice(
            Guid.NewGuid(),
            "Test choice",
            new ChoiceTransition(targetLabel)
        );
        
        original.AddChoice(choice);

        // Act
        var dbo = _mapper.ToDbO(original);
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.Equal(original.Id, result.Id);
        Assert.Equal(original.Choices.Count, result.Choices.Count);
        Assert.Equal(original.Choices[0].Id, result.Choices[0].Id);
        Assert.Equal(original.Choices[0].Text, result.Choices[0].Text);
    }

    [Fact]
    public void ToDbO_MenuWithMultipleChoices_PreservesOrder()
    {
        // Arrange
        var menu = new Menu(Guid.NewGuid());
        var targetLabel = new Label(Guid.NewGuid(), "target", Guid.NewGuid());
        
        var choice1 = new Choice(Guid.NewGuid(), "First", new ChoiceTransition(targetLabel));
        var choice2 = new Choice(Guid.NewGuid(), "Second", new ChoiceTransition(targetLabel));
        var choice3 = new Choice(Guid.NewGuid(), "Third", new ChoiceTransition(targetLabel));
        
        menu.AddChoice(choice1);
        menu.AddChoice(choice2);
        menu.AddChoice(choice3);

        // Act
        var result = _mapper.ToDbO(menu);

        // Assert
        Assert.Equal(3, result.Choices.Count);
        Assert.Equal("First", result.Choices[0].Text);
        Assert.Equal("Second", result.Choices[1].Text);
        Assert.Equal("Third", result.Choices[2].Text);
    }

    [Fact]
    public void ToDomain_MenuDbO_WithLongChoiceText_PreservesText()
    {
        // Arrange
        var longText = new string('A', 1000);
        var dbo = new MenuDbO
        {
            Id = Guid.NewGuid(),
            Choices =
            [
                new ChoiceDbO
                {
                    Id = Guid.NewGuid(),
                    Text = longText,
                    MenuId = Guid.NewGuid(),
                    NextLabelId = Guid.NewGuid(),
                    NextLabel = new LabelDbO
                    {
                        Id = Guid.NewGuid(),
                        LabelName = "target",
                        NovelId = Guid.NewGuid(),
                        Steps = []
                    }
                }
            ]
        };

        // Act
        var result = _mapper.ToDomain(dbo);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Choices);
        Assert.Equal(longText, result.Choices[0].Text);
        Assert.Equal(1000, result.Choices[0].Text.Length);
    }

    [Fact]
    public void ToDbO_MenuWithSpecialCharactersInChoiceText_PreservesText()
    {
        // Arrange
        var menu = new Menu(Guid.NewGuid());
        var targetLabel = new Label(Guid.NewGuid(), "target", Guid.NewGuid());
        
        var specialText = "Choice with \"quotes\" and 'apostrophes' and \n newlines";
        var choice = new Choice(
            Guid.NewGuid(),
            specialText,
            new ChoiceTransition(targetLabel)
        );
        
        menu.AddChoice(choice);

        // Act
        var result = _mapper.ToDbO(menu);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Choices);
        Assert.Equal(specialText, result.Choices[0].Text);
    }
}

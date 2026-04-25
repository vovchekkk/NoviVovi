using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Labels.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Scene.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Core.Statements.Mappers;
using NoviVovi.Infrastructure.Exporters.RenPy.Services.Utilities;

namespace NoviVovi.Infrastructure.Tests.Exporters.RenPy.Mappers;

public class LabelToRenPyMapperTests
{
    private readonly RenPyIdentifierGenerator _idGenerator;
    private readonly TransformToRenPyMapper _transformMapper;
    private readonly StepToRenPyMapper _stepMapper;
    private readonly LabelToRenPyMapper _mapper;

    public LabelToRenPyMapperTests()
    {
        _idGenerator = new RenPyIdentifierGenerator();
        _transformMapper = new TransformToRenPyMapper();
        _stepMapper = new StepToRenPyMapper(_idGenerator, _transformMapper);
        _mapper = new LabelToRenPyMapper(_idGenerator, _stepMapper);
    }

    [Fact]
    public void Map_ValidLabel_ReturnsRenPyLabel()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = Label.Create("start", novelId);

        // Act
        var result = _mapper.Map(label);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("label_", result.Identifier);
        Assert.NotNull(result.Statements);
        Assert.Empty(result.Statements);
    }

    [Fact]
    public void Map_LabelWithSteps_MapsAllSteps()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = Label.Create("chapter1", novelId);
        
        var targetLabel = Label.Create("chapter2", novelId);
        var jumpStep = JumpStep.Create(targetLabel);
        label.AddStep(jumpStep);

        // Act
        var result = _mapper.Map(label);

        // Assert
        Assert.Single(result.Statements);
    }

    [Fact]
    public void Map_SameLabelTwice_ReturnsSameIdentifier()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = Label.Create("test", novelId);

        // Act
        var result1 = _mapper.Map(label);
        var result2 = _mapper.Map(label);

        // Assert
        Assert.Equal(result1.Identifier, result2.Identifier);
    }

    [Fact]
    public void Map_DifferentLabels_ReturnsDifferentIdentifiers()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label1 = Label.Create("label1", novelId);
        var label2 = Label.Create("label2", novelId);

        // Act
        var result1 = _mapper.Map(label1);
        var result2 = _mapper.Map(label2);

        // Assert
        Assert.NotEqual(result1.Identifier, result2.Identifier);
    }

    [Fact]
    public void Map_LabelWithSpecialCharactersInName_MapsSuccessfully()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = Label.Create("Глава 1: Начало", novelId);

        // Act
        var result = _mapper.Map(label);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("label_", result.Identifier);
    }

    [Fact]
    public void Map_LabelWithLongName_MapsSuccessfully()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var longName = new string('A', 200);
        var label = Label.Create(longName, novelId);

        // Act
        var result = _mapper.Map(label);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("label_", result.Identifier);
    }

    [Fact]
    public void Map_Identifier_IsValidPythonIdentifier()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = Label.Create("test", novelId);

        // Act
        var result = _mapper.Map(label);

        // Assert
        Assert.Matches(@"^label_[a-f0-9]{32}$", result.Identifier);
    }

    [Fact]
    public void Map_EmptyLabel_ReturnsEmptyStatementsList()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = Label.Create("empty", novelId);

        // Act
        var result = _mapper.Map(label);

        // Assert
        Assert.Empty(result.Statements);
    }

    [Fact]
    public void Map_LabelWithMultipleSteps_PreservesOrder()
    {
        // Arrange
        var novelId = Guid.NewGuid();
        var label = Label.Create("multi", novelId);
        
        var target1 = Label.Create("target1", novelId);
        var target2 = Label.Create("target2", novelId);
        
        var step1 = JumpStep.Create(target1);
        var step2 = JumpStep.Create(target2);
        
        label.AddStep(step1);
        label.AddStep(step2);

        // Act
        var result = _mapper.Map(label);

        // Assert
        Assert.Equal(2, result.Statements.Count);
    }
}

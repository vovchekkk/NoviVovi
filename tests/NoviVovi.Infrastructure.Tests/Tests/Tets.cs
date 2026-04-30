using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.DatabaseObjects.Choices;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests.Tests;

public class LabelMapperTests
{
    [Fact]
    public void ToDomain_Should_Map_Without_Cycles()
    {
        var provider = TestHelper.CreateProvider();
        var mapper = provider.GetRequiredService<LabelMapper>();

        var labelId = Guid.NewGuid();

        var label = new LabelDbO
        {
            Id = labelId,
            LabelName = "Test",
            Steps = []
        };

        var step = new StepDbO
        {
            Id = Guid.NewGuid(),
            LabelId = labelId,
            StepType = "jump",
            NextLabelId = labelId,
            NextLabel = label
        };

        label.Steps.Add(step);

        var result = mapper.ToDomain(label);

        Assert.NotNull(result);
        Assert.Equal(labelId, result.Id);
        Assert.Single(result.Steps);
    }
}

public class MenuMapperTests
{
    [Fact]
    public void ToDomain_Should_Map_Choices()
    {
        var provider = TestHelper.CreateProvider();
        var mapper = provider.GetRequiredService<MenuMapper>();

        var label = new LabelDbO
        {
            Id = Guid.NewGuid(),
            LabelName = "Next"
        };

        var menu = new MenuDbO
        {
            Id = Guid.NewGuid(),
            Choices = new List<ChoiceDbO>
            {
                new ChoiceDbO
                {
                    Id = Guid.NewGuid(),
                    Text = "Go",
                    NextLabel = label,
                    NextLabelId = label.Id
                }
            }
        };

        var result = mapper.ToDomain(menu);

        Assert.NotNull(result);
        Assert.Single(result.Choices);
    }
}

public class StepMapperTests
{
    [Fact]
    public void ToDomain_JumpStep_Should_Map()
    {
        var provider = TestHelper.CreateProvider();
        var mapper = provider.GetRequiredService<StepMapper>();

        var label = new LabelDbO
        {
            Id = Guid.NewGuid(),
            LabelName = "Target"
        };

        var step = new StepDbO
        {
            Id = Guid.NewGuid(),
            StepType = "jump",
            NextLabel = label,
            NextLabelId = label.Id
        };

        var result = mapper.ToDomain(step);

        Assert.NotNull(result);
        Assert.IsType<JumpStep>(result);
    }
    
    [Fact]
    public void Mapping_Should_Not_StackOverflow_On_Cycle()
    {
        var provider = TestHelper.CreateProvider();
        var mapper = provider.GetRequiredService<LabelMapper>();

        var label1 = new LabelDbO { Id = Guid.NewGuid(), LabelName = "L1" };
        var label2 = new LabelDbO { Id = Guid.NewGuid(), LabelName = "L2" };

        var step1 = new StepDbO
        {
            Id = Guid.NewGuid(),
            StepType = "jump",
            NextLabel = label2,
            NextLabelId = label2.Id
        };

        var step2 = new StepDbO
        {
            Id = Guid.NewGuid(),
            StepType = "jump",
            NextLabel = label1,
            NextLabelId = label1.Id
        };

        label1.Steps = [step1];
        label2.Steps = [step2];

        var result = mapper.ToDomain(label1);

        Assert.NotNull(result);
    }
}
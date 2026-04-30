using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests.Tests;

public class TestMapping(LabelMapper mapper)
{
    public void Test1()
    {
        var bgId = Guid.NewGuid();
        
        var label1Id = Guid.NewGuid();
        var label2Id = Guid.NewGuid();
        
        var label1 = new LabelDbO()
        {
            Id = label1Id,
            LabelName = "Label1",
            Steps = new List<StepDbO>()
        };

        var label2 = new LabelDbO()
        {
            Id = label2Id,
            LabelName = "Label2",
            Steps = new List<StepDbO>()
        };

        var step1 = new StepDbO
        {
            Id = Guid.NewGuid(),
            StepOrder = 1,
            Background = new BackgroundDbO
            {
                Id = Guid.NewGuid(),
                Image = new ImageDbO()
                {
                    Id = bgId,
                    Format = "PNG",
                    Height = 100,
                    Width = 100,
                    ImgType = "background",
                    Name = "background",
                    NovelId = Guid.Empty,
                    Size = 1,
                    Url = "aboba/chinazes/12"
                },
                Img = bgId,
                Transform = null,
                TransformId = null
            },
            StepType = "background"
        };

        var step2 = new StepDbO
        {
            Id = Guid.NewGuid(),
            NextLabelId = label2Id,
            NextLabel = label2,
            StepType = "jump"
        };
        
        label1.Steps.Add(step1);
        label1.Steps.Add(step2);
        
        var res = mapper.ToDomain(label1);
        Console.WriteLine(res);
    }
}
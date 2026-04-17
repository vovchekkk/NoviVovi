using Microsoft.Extensions.DependencyInjection;
using NoviVovi.Infrastructure.DatabaseObjects.Images;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using NoviVovi.Infrastructure.Mappers;

namespace NoviVovi.Infrastructure.Tests;

public class TestMapping(LabelMapper mapper)
{
    public void Test1()
    {
        var bgId = Guid.NewGuid();
        
        var Label1Id = Guid.NewGuid();
        var Label2Id = Guid.NewGuid();
        
        var Label1 = new LabelDbO()
        {
            Id = Label1Id,
            LabelName = "Label1",
            Steps = new List<StepDbO>()
        };

        var Label2 = new LabelDbO()
        {
            Id = Label2Id,
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
            NextLabelId = Label2Id,
            NextLabel = Label2,
            StepType = "jump"
        };
        
        Label1.Steps.Add(step1);
        Label1.Steps.Add(step2);
        
        var res = mapper.ToDomain(Label1, new MappingContext());
        Console.WriteLine(res);
    }
    
    public static void RunTest()
    {
        var services = new ServiceCollection();
        
        services.AddScoped<LabelMapper>();
        services.AddScoped<StepMapper>();
        services.AddScoped<MenuMapper>();
        services.AddScoped<CharacterMapper>();
        services.AddScoped<ImageMapper>();
        services.AddScoped<ReplicaMapper>();
        services.AddScoped<TransformMapper>();

        var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();

        var mapper = scope.ServiceProvider.GetRequiredService<LabelMapper>();

        var test = new TestMapping(mapper);
        test.Test1();
    }
}
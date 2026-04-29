using NoviVovi.Domain.Labels;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class LabelMapper(
    Lazy<StepMapper> stepMapper,
    MappingContext ctx
)
{
    public Label ToDomain(LabelDbO dbo)
    {
        if (ctx.Labels.TryGetValue(dbo.Id, out var cached))
            return cached;

        var res = new Label(dbo.Id, dbo.LabelName, dbo.NovelId);
        ctx.Labels[dbo.Id] = res;

        foreach (var step in dbo.Steps)
        {
            res.AddStep(stepMapper.Value.ToDomain(step));
        }

        return res;
    }

    public LabelDbO? ToDbO(Label label)
    {
        if(label == null)
            return null;
        
        if (ctx.LabelDbOs.TryGetValue(label.Id, out var cached))
            return cached;

        var res = new LabelDbO
        {
            Id = label.Id,
            LabelName = label.Name,
            NovelId = label.NovelId
        };

        ctx.LabelDbOs[label.Id] = res;

        res.Steps = label.Steps
            .Select((step, i) => stepMapper.Value.ToDbO(step, label.Id, label.NovelId, i))
            .ToList();

        return res;
    }
}
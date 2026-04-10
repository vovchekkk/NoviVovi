using NoviVovi.Domain.Labels;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class LabelMapper(StepMapper stepMapper)
{
    public Label ToDomain(LabelDbO dbo)
    {
        var res = new Label(dbo.Id, dbo.LabelName);
        foreach (var steps in dbo.Steps)
        {
            res.AddStep(stepMapper.ToDomain(steps));
        }
        return res;
    }

    public LabelDbO ToDbO(Label label, Guid novelId)
    {
        var res = new LabelDbO
        {
            Id = label.Id,
            LabelName = label.Name,
            NovelId = novelId
        };
        res.Steps = label.Steps.Select((step, i) => stepMapper.ToDbO(step, label.Id, novelId, i)).ToList();
        return res;
    }
}
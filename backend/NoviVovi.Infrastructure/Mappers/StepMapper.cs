using NoviVovi.Domain.Steps;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;


[Mapper]
public partial class StepMapper
{
    public StepDbO ToDbO(Step step, Guid novelId)
    {
        throw new NotImplementedException();
    }
}
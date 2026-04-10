using NoviVovi.Domain.Labels;
using NoviVovi.Infrastructure.DatabaseObjects.Labels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class LabelMapper
{
    public Label? ToDomain(LabelDbO dbo)
    {
        throw new NotImplementedException();
    }

    public LabelDbO ToDbO(Label label)
    {
        throw new NotImplementedException();
    }
}
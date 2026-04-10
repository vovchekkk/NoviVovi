using NoviVovi.Domain.Novels;
using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class NovelsMapper
{
    public partial NovelDbO ToDbO(Novel novel);

    public Novel? ToDomain(object dbo)
    {
        throw new NotImplementedException();
    }
}
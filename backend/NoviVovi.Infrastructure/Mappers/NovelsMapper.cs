using NoviVovi.Domain.Novels;
using NoviVovi.Infrastructure.DatabaseObjects.Novels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Mappers;

[Mapper]
public partial class NovelsMapper
{
    public Novel ToNovel(FullNovelDbO novelDbo)
    {
        throw new NotImplementedException();
    }
    
    public partial FullNovelDbO ToDbO(Novel novel);

}
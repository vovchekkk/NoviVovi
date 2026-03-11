using NoviVovi.Domain.Novels;
using NoviVovi.Infrastructure.Novels.Persistence;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Infrastructure.Novels.Mappers;

[Mapper]
public partial class NovelDbMapper
{
    public partial NovelDbModel ToSnapshot(Novel novel);

    public partial IEnumerable<NovelDbModel> ToSnapshots(IEnumerable<Novel> novels);
}
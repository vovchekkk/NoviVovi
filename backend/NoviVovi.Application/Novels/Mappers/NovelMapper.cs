using NoviVovi.Application.Novels.Contracts;
using NoviVovi.Domain.Novels;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Application.Novels.Mappers;

[Mapper]
public partial class NovelMapper
{
    public partial NovelSnapshot ToSnapshot(Novel subject);
    
    public partial IEnumerable<NovelSnapshot> ToSnapshots(IEnumerable<Novel> subjects);
}
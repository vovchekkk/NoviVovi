using NoviVovi.Api.Novels.Responses;
using NoviVovi.Application.Novels.Contracts;
using Riok.Mapperly.Abstractions;

namespace NoviVovi.Api.Novels.Mappers;

[Mapper]
public partial class NovelResponseMapper
{
    public partial NovelResponse ToSnapshot(NovelSnapshot novel);
    
    public partial IEnumerable<NovelResponse> ToSnapshots(IEnumerable<NovelSnapshot> novels);
}
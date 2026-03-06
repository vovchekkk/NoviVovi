using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Contracts.Novels;

namespace NoviVovi.Application.Features.Novels.Get;

public class GetNovelHandler(INovelRepository repo)
{
    public async Task<NovelSnapshot?> Handle(GetNovelQuery query)
    {
        var novel = await repo.GetById(query.Id);
        
        return novel?.ToDto();
    }
}
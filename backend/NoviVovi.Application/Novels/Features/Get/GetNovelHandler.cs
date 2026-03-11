using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Novels.Contracts;

namespace NoviVovi.Application.Novels.Features.Get;

public class GetNovelHandler(INovelRepository repo)
{
    public async Task<NovelSnapshot?> Handle(GetNovelQuery query)
    {
        var novel = await repo.GetById(query.Id);
        
        return novel?.ToDto();
    }
}
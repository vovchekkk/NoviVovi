using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Novels.DTO;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels.Get;

public class GetNovelHandler(INovelRepository repo)
{
    public async Task<NovelDto?> Handle(GetNovelQuery query)
    {
        var novel = await repo.GetById(query.Id);
        
        return novel?.ToDto();
    }
}
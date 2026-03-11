using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Novels.Contracts;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels.Features.Create;

public class CreateNovelHandler(INovelRepository repo)
{
    public async Task<NovelSnapshot> Handle(CreateNovelCommand cmd)
    {
        var novel = Novel.Create(cmd.Title);
        
        await repo.Save(novel);
        
        return novel.ToDto();
    }
}
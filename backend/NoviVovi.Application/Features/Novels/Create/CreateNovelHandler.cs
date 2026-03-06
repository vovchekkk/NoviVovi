using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Contracts.Novels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Features.Novels.Create;

public class CreateNovelHandler(INovelRepository repo)
{
    public async Task<NovelSnapshot> Handle(CreateNovelCommand cmd)
    {
        var novel = Novel.Create(cmd.Title);
        
        await repo.Save(novel);
        
        return novel.ToDto();
    }
}
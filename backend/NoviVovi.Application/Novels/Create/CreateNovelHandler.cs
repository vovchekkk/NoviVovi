using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Novels.DTO;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels.Create;

public class CreateNovelHandler(INovelRepository repo)
{
    public async Task<NovelDto> Handle(CreateNovelCommand cmd)
    {
        var novel = Novel.Create(cmd.Title);
        
        await repo.Save(novel);
        
        return novel.ToDto();
    }
}
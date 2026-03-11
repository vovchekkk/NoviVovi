using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Novels.Contracts;
using NoviVovi.Application.Novels.Mappers;

namespace NoviVovi.Application.Novels.Features.Get;

public class GetNovelHandler(
    INovelRepository repository,
    NovelMapper mapper
)
{
    public async Task<NovelSnapshot?> Handle(GetNovelQuery query)
    {
        var novel = await repository.GetByIdAsync(query.Id);

        return mapper.ToSnapshot(novel);
    }
}
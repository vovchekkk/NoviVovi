using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Contracts;
using NoviVovi.Application.Novels.Mappers;

namespace NoviVovi.Application.Novels.Features.Get;

public class GetNovelHandler(
    INovelRepository novelRepository,
    NovelMapper mapper
)
{
    public async Task<NovelSnapshot?> Handle(GetNovelQuery query)
    {
        var novel = await novelRepository.GetByIdAsync(query.NovelId);
        if (novel == null)
            throw new NotFoundException($"Новелла с ID '{query.NovelId}' не найдена");

        return mapper.ToSnapshot(novel);
    }
}
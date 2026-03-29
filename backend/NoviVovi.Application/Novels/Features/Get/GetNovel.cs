using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Contracts;
using NoviVovi.Application.Novels.Mappers;

namespace NoviVovi.Application.Novels.Features.Get;

public record GetNovelQuery(
    Guid NovelId
) : IRequest<NovelSnapshot>;

public class GetNovelHandler(
    INovelRepository novelRepository,
    NovelSnapshotMapper snapshotMapper
)
{
    public async Task<NovelSnapshot?> Handle(GetNovelQuery query)
    {
        var novel = await novelRepository.GetByIdAsync(query.NovelId);
        if (novel == null)
            throw new NotFoundException($"Новелла с ID '{query.NovelId}' не найдена");

        return snapshotMapper.ToSnapshot(novel);
    }
}
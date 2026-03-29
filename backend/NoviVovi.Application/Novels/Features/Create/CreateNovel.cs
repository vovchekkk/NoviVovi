using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels.Contracts;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels.Features.Create;

public record CreateNovelCommand(
    string Title,
    Guid StartLabel
) : IRequest<NovelSnapshot>;

public class CreateNovelHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    NovelSnapshotMapper snapshotMapper
)
{
    public async Task<NovelSnapshot> Handle(CreateNovelCommand command)
    {
        var startLabel = Label.Create("Start");

        var novel = Novel.Create(command.Title, startLabel);

        await labelRepository.AddAsync(startLabel);

        await novelRepository.AddAsync(novel);

        return snapshotMapper.ToSnapshot(novel);
    }
}
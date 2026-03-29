using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels.Features.Create;

public record CreateNovelCommand(
    string Title,
    Guid StartLabel
) : IRequest<NovelDto>;

public class CreateNovelHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    NovelDtoMapper dtoMapper
) : IRequestHandler<CreateNovelCommand, NovelDto>
{
    public async Task<NovelDto> Handle(CreateNovelCommand request, CancellationToken cancellationToken)
    {
        var startLabel = Label.Create("Start");

        var novel = Novel.Create(request.Title, startLabel);

        await labelRepository.AddAsync(startLabel);

        await novelRepository.AddAsync(novel);

        return dtoMapper.ToDto(novel);
    }
}
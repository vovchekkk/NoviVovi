using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Novels.Features.Create;

public record CreateNovelCommand : IRequest<NovelDto>
{
    public required string Title { get; init; }
}

public class CreateNovelHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    NovelDtoMapper mapper
) : IRequestHandler<CreateNovelCommand, NovelDto>
{
    public async Task<NovelDto> Handle(CreateNovelCommand request, CancellationToken ct)
    {
        var startLabel = Label.Create("Start");

        var novel = Novel.Create(request.Title, startLabel);

        await labelRepository.AddAsync(startLabel, novel.Id , ct);
        await novelRepository.AddAsync(novel, ct);
        
        await unitOfWork.SaveChangesAsync(ct);
        
        return mapper.ToDto(novel);
    }
}
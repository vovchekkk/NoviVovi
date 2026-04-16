using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Novels.Features.Patch;

public record PatchNovelCommand : IRequest<NovelDto>
{
    public required Guid NovelId { get; init; }
    public string? Title { get; init; }
    public Guid? StartLabelId { get; init; }
}

public class PatchNovelHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    NovelDtoMapper mapper
) : IRequestHandler<PatchNovelCommand, NovelDto>
{
    public async Task<NovelDto> Handle(PatchNovelCommand request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");
        
        if (request.Title != null)
            novel.UpdateTitle(request.Title);
        
        if (request.StartLabelId != null)
        {
             var label = await labelRepository.GetByIdAsync(request.StartLabelId.Value, ct)
                         ?? throw new NotFoundException($"Метка '{request.StartLabelId}' не найдена");
             
             novel.SetStartLabel(label);
        }

        await unitOfWork.SaveChangesAsync(ct);
        
        return mapper.ToDto(novel);
    }
}
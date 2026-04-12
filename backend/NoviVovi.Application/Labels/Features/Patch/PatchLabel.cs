using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Features.Add;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels;

namespace NoviVovi.Application.Labels.Features.Patch;

public record PatchLabelCommand : IRequest<LabelDto>
{
    public required Guid NovelId { get; init; }
    public required Guid LabelId { get; init; }
    public string? Name { get; init; }
}

public class PatchLabelHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    LabelDtoMapper mapper
) : IRequestHandler<PatchLabelCommand, LabelDto>
{
    public async Task<LabelDto> Handle(PatchLabelCommand request, CancellationToken ct)
    {
        var label = await labelRepository.GetByIdAsync(request.LabelId, ct)
                    ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");
        
        if (label.NovelId != request.NovelId)
            throw new ConflictException($"Метка '{request.LabelId}' не принадлежит новелле '{request.NovelId}'");
        
        if (request.Name != null)
            label.UpdateName(request.Name);
        
        await unitOfWork.SaveChangesAsync(ct);
        
        return mapper.ToDto(label);
    }
}
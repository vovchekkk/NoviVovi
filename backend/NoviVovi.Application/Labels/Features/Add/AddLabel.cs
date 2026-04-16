using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Abstractions;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Domain.Labels;

namespace NoviVovi.Application.Labels.Features.Add;

public record AddLabelCommand : IRequest<LabelDto>
{
    public required Guid NovelId { get; init; }
    public required string Name { get; init; }
}

public class AddLabelHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    IUnitOfWork unitOfWork,
    LabelDtoMapper mapper
) : IRequestHandler<AddLabelCommand, LabelDto>
{
    public async Task<LabelDto> Handle(AddLabelCommand request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");
        
        var label = Label.Create(request.Name, request.NovelId);
        
        novel.AddLabel(label);
        
        await labelRepository.AddAsync(label, ct);
        
        await unitOfWork.SaveChangesAsync(ct);
        
        return mapper.ToDto(label);
    }
}
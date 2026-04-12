using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Steps.Features.Add;

public abstract record AddStepCommand : IRequest<StepDto>
{
    public required Guid NovelId { get; init; }
    public required Guid LabelId { get; init; }
}

public abstract class BaseAddStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository)
{
    protected readonly INovelRepository NovelRepository = novelRepository;
    protected readonly ILabelRepository LabelRepository = labelRepository;
    
    protected async Task<(Novel, Label)> GetStepContextOrThrow(AddStepCommand request, CancellationToken ct)
    {
        var novel = await NovelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var label = await LabelRepository.GetByIdAsync(request.LabelId, ct)
                    ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");
        
        if (label.NovelId != request.NovelId)
            throw new ConflictException($"Метка '{request.LabelId}' не принадлежит новелле '{request.NovelId}'");

        return (novel, label);
    }
}
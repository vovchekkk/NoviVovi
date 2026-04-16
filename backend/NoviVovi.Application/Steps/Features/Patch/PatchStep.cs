using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Domain.Labels;
using NoviVovi.Domain.Novels;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Steps.Features.Patch;

public abstract record PatchStepCommand : IRequest<StepDto>
{
    public required Guid NovelId { get; init; }
    public required Guid LabelId { get; init; }
    public required Guid StepId { get; init; }
}

public abstract class BasePatchStepHandler(
    ILabelRepository labelRepository)
{
    protected readonly ILabelRepository LabelRepository = labelRepository;
    
    protected async Task<Step> GetStepContextOrThrow(PatchStepCommand request, CancellationToken ct)
    {
        var label = await LabelRepository.GetByIdAsync(request.LabelId, ct)
                    ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");
        
        if (label.NovelId != request.NovelId)
            throw new ConflictException($"Метка '{request.LabelId}' не принадлежит новелле '{request.NovelId}'");

        var step = label.Steps.FirstOrDefault(s => s.Id == request.StepId)
                   ?? throw new NotFoundException($"Шаг '{request.StepId}' не найден в метке");

        return step;
    }
}
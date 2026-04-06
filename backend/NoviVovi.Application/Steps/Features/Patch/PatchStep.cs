using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
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
    INovelRepository novelRepository,
    ILabelRepository labelRepository)
{
    protected async Task<(Novel, Label, Step)> GetStepContextOrThrow(PatchStepCommand request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var label = await labelRepository.GetByIdAsync(request.LabelId, ct)
                    ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");

        var step = label.Steps.FirstOrDefault(s => s.Id == request.StepId)
                   ?? throw new NotFoundException($"Шаг '{request.StepId}' не найден в метке");

        return (novel, label, step);
    }
}
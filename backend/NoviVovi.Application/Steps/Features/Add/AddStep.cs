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
    protected async Task<(Novel, Label)> GetStepContextOrThrow(AddStepCommand request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var label = await labelRepository.GetByIdAsync(request.LabelId, ct)
                    ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");

        return (novel, label);
    }
}
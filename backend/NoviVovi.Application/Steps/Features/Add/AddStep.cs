using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
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
    ILabelRepository labelRepository
)
{
    protected readonly ILabelRepository LabelRepository = labelRepository;

    protected async Task<Label> GetStepContextOrThrow(AddStepCommand request, CancellationToken ct)
    {
        var label = await LabelRepository.GetByIdAsync(request.LabelId, ct)
                    ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");

        if (label.NovelId != request.NovelId)
            throw new ConflictException($"Метка '{request.LabelId}' не принадлежит новелле '{request.NovelId}'");

        return label;
    }
}
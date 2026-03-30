using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;

namespace NoviVovi.Application.Steps.Features.Get;

public record GetStepQuery(
    Guid NovelId,
    Guid LabelId,
    Guid StepId
) : IRequest<StepDto>;

public class GetStepHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : IRequestHandler<GetStepQuery, StepDto>
{
    public async Task<StepDto> Handle(GetStepQuery request, CancellationToken cancellationToken)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId)
                    ?? throw new NotFoundException($"Новелла {request.NovelId} не найдена");

        var label = await labelRepository.GetByIdAsync(request.LabelId)
                    ?? throw new NotFoundException($"Метка {request.LabelId} не найдена");

        var step = label.Steps.FirstOrDefault(s => s.Id == request.StepId)
                   ?? throw new NotFoundException($"Шаг {request.StepId} не найден");

        return mapper.ToDto(step);
    }
}
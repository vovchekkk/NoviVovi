using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Steps.Dtos;
using NoviVovi.Application.Steps.Mappers;

namespace NoviVovi.Application.Steps.Features.Get;

public record GetStepsQuery(
    Guid NovelId,
    Guid LabelId
) : IRequest<IEnumerable<StepDto>>;

public class GetStepsHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    StepDtoMapper mapper
) : IRequestHandler<GetStepsQuery, IEnumerable<StepDto>>
{
    public async Task<IEnumerable<StepDto>> Handle(GetStepsQuery request, CancellationToken ct)
    {
        var label = await labelRepository.GetByIdAsync(request.LabelId, ct)
                    ?? throw new NotFoundException($"Метка {request.LabelId} не найдена");
        
        if (label.NovelId != request.NovelId)
            throw new ConflictException($"Метка '{request.LabelId}' не принадлежит новелле '{request.NovelId}'");

        var steps = label.Steps;
        
        return mapper.ToDtos(steps);
    }
}
using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Labels.Abstractions;
using NoviVovi.Application.Preview.Dtos;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Domain.Preview;
using NoviVovi.Domain.Steps;

namespace NoviVovi.Application.Preview.Features.Get;

public record GetScenePreviewQuery(
    Guid NovelId,
    Guid LabelId,
    Guid StepId
) : IRequest<SceneStateDto>;

public class GetScenePreviewHandler(
    ILabelRepository labelRepository,
    SceneStateDtoMapper mapper
) : IRequestHandler<GetScenePreviewQuery, SceneStateDto>
{
    public async Task<SceneStateDto> Handle(GetScenePreviewQuery request, CancellationToken ct)
    {
        var label = await labelRepository.GetByIdAsync(request.LabelId, ct)
                    ?? throw new NotFoundException($"Метка '{request.LabelId}' не найдена");
        
        if (label.NovelId != request.NovelId)
            throw new ConflictException($"Метка '{request.LabelId}' не принадлежит новелле '{request.NovelId}'");

        var snapshot = new VisualSnapshot();
        foreach (var step in label.GetStepsUntil(request.StepId))
        {
            snapshot.Apply(step);
        }

        return mapper.ToDto(snapshot);
    }
}
using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Preview.Dtos;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Domain.Preview;

namespace NoviVovi.Application.Preview.Features.Get;

public record GetScenePreviewQuery(
    Guid LabelId,
    Guid StepId
) : IRequest<SceneStateDto>;

public class GetScenePreviewHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    SceneStateDtoMapper mapper
) : IRequestHandler<GetScenePreviewQuery, SceneStateDto>
{
    public async Task<SceneStateDto> Handle(GetScenePreviewQuery request, CancellationToken ct)
    {
        var label = await labelRepository.GetByIdAsync(request.LabelId, ct);
        if (label == null)
            throw new NotFoundException($"Метка '{request.LabelId}' не найдена");

        // Создаем чистый снимок
        var snapshot = new VisualSnapshot();

        // Проигрываем все шаги ДО выбранного включительно
        // foreach (var step in label.Steps.TakeWhileBefore(request.StepId))
        // {
        //     snapshot.Apply(step);
        // }

        return mapper.ToDto(snapshot);
    }
}
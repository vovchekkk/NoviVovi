using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Labels.Dtos;
using NoviVovi.Application.Labels.Features.Add;
using NoviVovi.Application.Labels.Mappers;
using NoviVovi.Application.Novels;

namespace NoviVovi.Application.Labels.Features.Get;

public record GetLabelsQuery(
    Guid NovelId
) : IRequest<IEnumerable<LabelDto>>;

public class GetLabelsHandler(
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    LabelDtoMapper mapper
) : IRequestHandler<GetLabelsQuery, IEnumerable<LabelDto>>
{
    public async Task<IEnumerable<LabelDto>> Handle(GetLabelsQuery request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        var labels = novel.Labels;
        
        return mapper.ToDtos(labels);
    }
}
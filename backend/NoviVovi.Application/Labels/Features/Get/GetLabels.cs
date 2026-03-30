using MediatR;
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
    public Task<IEnumerable<LabelDto>> Handle(GetLabelsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
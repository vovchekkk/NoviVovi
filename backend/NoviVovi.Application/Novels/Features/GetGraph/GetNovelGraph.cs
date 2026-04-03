using MediatR;
using NoviVovi.Application.Novels.Dtos;

namespace NoviVovi.Application.Novels.Features.GetGraph;

public class GetNovelGraphQuery(
    Guid NovelId
) : IRequest<NovelGraphDto>;

public class GetNovelGraphHandler(
    INovelRepository novelRepository
) : IRequestHandler<GetNovelGraphQuery, NovelGraphDto>
{
    public async Task<NovelGraphDto> Handle(GetNovelGraphQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
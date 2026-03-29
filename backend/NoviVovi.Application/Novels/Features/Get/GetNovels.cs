using MediatR;
using NoviVovi.Application.Novels.Dtos;

namespace NoviVovi.Application.Novels.Features.Get;

public record GetNovelsQuery : IRequest<IEnumerable<NovelDto>>
{
    
}

public class GetNovelsHandler : IRequestHandler<GetNovelsQuery, IEnumerable<NovelDto>>
{
    public async Task<IEnumerable<NovelDto>> Handle(GetNovelsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
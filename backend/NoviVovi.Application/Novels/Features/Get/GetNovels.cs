using MediatR;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;

namespace NoviVovi.Application.Novels.Features.Get;

public record GetNovelsQuery : IRequest<IEnumerable<NovelDto>>
{
}

public class GetNovelsHandler(
    INovelRepository novelRepository,
    NovelDtoMapper mapper
) : IRequestHandler<GetNovelsQuery, IEnumerable<NovelDto>>
{
    public async Task<IEnumerable<NovelDto>> Handle(GetNovelsQuery request, CancellationToken ct)
    {
        var novels = await novelRepository.GetAllAsync(ct);
        
        return mapper.ToDtos(novels);
    }
}
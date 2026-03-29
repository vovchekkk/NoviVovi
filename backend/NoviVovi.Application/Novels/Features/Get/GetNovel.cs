using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;

namespace NoviVovi.Application.Novels.Features.Get;

public record GetNovelQuery(
    Guid NovelId
) : IRequest<NovelDto>;

public class GetNovelHandler(
    INovelRepository novelRepository,
    NovelDtoMapper dtoMapper
) : IRequestHandler<GetNovelQuery, NovelDto>
{
    public async Task<NovelDto> Handle(GetNovelQuery request, CancellationToken cancellationToken)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId);
        if (novel == null)
            throw new NotFoundException($"Новелла с ID '{request.NovelId}' не найдена");

        return dtoMapper.ToDto(novel);
    }
}
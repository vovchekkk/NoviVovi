using MediatR;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;

namespace NoviVovi.Application.Novels.Features.Get;

public record GetNovelQuery(
    Guid NovelId
) : IRequest<NovelDto>;

public class GetNovelHandler(
    INovelRepository novelRepository,
    NovelDtoMapper mapper
) : IRequestHandler<GetNovelQuery, NovelDto>
{
    public async Task<NovelDto> Handle(GetNovelQuery request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        return mapper.ToDto(novel);
    }
}
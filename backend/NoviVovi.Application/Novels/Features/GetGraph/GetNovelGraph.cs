using MediatR;
using NoviVovi.Application.Common;
using NoviVovi.Application.Common.Exceptions;
using NoviVovi.Application.Novels.Abstractions;
using NoviVovi.Application.Novels.Dtos;
using NoviVovi.Application.Novels.Mappers;
using NoviVovi.Application.Novels.Models;

namespace NoviVovi.Application.Novels.Features.GetGraph;

public record GetNovelGraphQuery(
    Guid NovelId
) : IRequest<NovelGraphDto>;

public class GetNovelGraphHandler(
    INovelRepository novelRepository,
    NovelGraphBuilder builder,
    NovelGraphDtoMapper mapper
) : IRequestHandler<GetNovelGraphQuery, NovelGraphDto>
{
    public async Task<NovelGraphDto> Handle(GetNovelGraphQuery request, CancellationToken ct)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId, ct)
                    ?? throw new NotFoundException($"Новелла '{request.NovelId}' не найдена");

        return mapper.ToDto(builder.Build(novel));
    }
}
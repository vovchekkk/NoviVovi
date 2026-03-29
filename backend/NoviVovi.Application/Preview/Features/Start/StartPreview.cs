using MediatR;
using NoviVovi.Application.Labels;
using NoviVovi.Application.Novels;
using NoviVovi.Application.Preview.Dtos;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Preview.Services;

namespace NoviVovi.Application.Preview.Features.Start;

public record StartPreviewCommand(
    Guid NovelId
) : IRequest<SceneStateDto>;

public class StartPreviewHandler(
    PreviewSessionStore sessions,
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    SceneStateDtoMapper stateDtoMapper
) : IRequestHandler<StartPreviewCommand, SceneStateDto>
{
    public async Task<SceneStateDto> Handle(StartPreviewCommand request, CancellationToken cancellationToken)
    {
        var novel = await novelRepository.GetByIdAsync(request.NovelId);
        if (novel == null)
        {
            throw new Exception($"Session with id {request.NovelId} not found.");
        }

        var session = await sessions.CreateAsync(novel);

        await session.Player.ExecuteNextAsync(labelRepository);

        await sessions.SaveAsync(session);

        return stateDtoMapper.ToDto(session.State);
    }
}
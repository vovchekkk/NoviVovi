using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Preview.Contracts;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Preview.Services;

namespace NoviVovi.Application.Preview.Features.Start;

public class StartPreviewHandler(
    PreviewSessionStore sessions,
    INovelRepository novelRepository,
    ILabelRepository labelRepository,
    SceneStateMapper stateMapper
)
{
    public async Task<SceneStateSnapshot> Handle(StartPreviewCommand command)
    {
        var novel = await novelRepository.GetByIdAsync(command.NovelId);
        if (novel == null)
        {
            throw new Exception($"Session with id {command.NovelId} not found.");
        }

        var session = await sessions.CreateAsync(novel);

        await session.Player.ExecuteNextAsync(labelRepository);

        await sessions.SaveAsync(session);

        return stateMapper.ToSnapshot(session.State);
    }
}
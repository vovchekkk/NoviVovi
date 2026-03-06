using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Contracts.Preview;
using NoviVovi.Application.Preview;

namespace NoviVovi.Application.Features.Preview.Start;

public class StartPreviewHandler
{
    private readonly INovelRepository _novels;
    private readonly PreviewSessionStore _sessions;

    public StartPreviewHandler(
        INovelRepository novels,
        PreviewSessionStore sessions)
    {
        _novels = novels;
        _sessions = sessions;
    }

    public async Task<SceneSnapshot> Handle(StartPreviewCommand command)
    {
        var novel = await _novels.Get(command.NovelId);

        var player = new ScenePlayer(novel);

        var sessionId = Guid.NewGuid();

        _sessions.Add(sessionId, player);

        player.ExecuteNext();

        return SceneSnapshot.From(player, sessionId);
    }
}
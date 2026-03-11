using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Mappers;
using NoviVovi.Application.Preview.Contracts;
using NovelMapper = NoviVovi.Application.Novels.Mappers.NovelMapper;

namespace NoviVovi.Application.Preview.Features.Start;

public class StartPreviewHandler(INovelRepository repository, NovelMapper mapper)
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
        var novel = await repository.GetByIdAsync(command.NovelId);
        
        var session = await _sessions.AddAsync(novel);

        session.Player.ExecuteNext();

        return SceneSnapshot.From(player, sessionId);
    }
}
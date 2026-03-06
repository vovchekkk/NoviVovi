using NoviVovi.Application.Contracts.Preview;
using NoviVovi.Application.Preview;

namespace NoviVovi.Application.Features.Preview.Next;

public class NextStepHandler
{
    private readonly PreviewSessionStore _sessions;

    public NextStepHandler(PreviewSessionStore sessions)
    {
        _sessions = sessions;
    }

    public Task<SceneSnapshot> Handle(NextStepCommand command)
    {
        var player = _sessions.Get(command.SessionId);

        player.ExecuteNext();

        return Task.FromResult(
            SceneSnapshot.From(player, command.SessionId)
        );
    }
}
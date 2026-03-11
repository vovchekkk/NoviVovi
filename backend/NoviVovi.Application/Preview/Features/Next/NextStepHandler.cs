using NoviVovi.Application.Preview.Contracts;

namespace NoviVovi.Application.Preview.Features.Next;

public class NextStepHandler
{
    private readonly PreviewSessionStore _sessions;

    public NextStepHandler(PreviewSessionStore sessions)
    {
        _sessions = sessions;
    }

    public Task<SceneSnapshot> Handle(NextStepCommand command)
    {
        var session = _sessions.GetByIDAsync(command.SessionId);

        session.Player.ExecuteNext();

        return Task.FromResult(
            SceneSnapshot.From(session.Player, command.SessionId)
        );
    }
}
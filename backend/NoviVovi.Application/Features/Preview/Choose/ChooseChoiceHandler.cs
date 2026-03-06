using NoviVovi.Application.Contracts.Preview;
using NoviVovi.Application.Preview;

namespace NoviVovi.Application.Features.Preview.Choose;

public class ChooseChoiceHandler
{
    private readonly PreviewSessionStore _sessions;

    public ChooseChoiceHandler(PreviewSessionStore sessions)
    {
        _sessions = sessions;
    }

    public Task<SceneSnapshot> Handle(ChooseChoiceCommand command)
    {
        var player = _sessions.Get(command.SessionId);

        player.Player.SelectChoice(command.ChoiceId);

        player.ExecuteNext();

        return Task.FromResult(
            SceneSnapshot.From(player, command.SessionId)
        );
    }
}
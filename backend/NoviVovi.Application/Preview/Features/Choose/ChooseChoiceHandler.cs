using NoviVovi.Application.Preview.Contracts;

namespace NoviVovi.Application.Preview.Features.Choose;

public class ChooseChoiceHandler
{
    private readonly PreviewSessionStore _sessions;

    public ChooseChoiceHandler(PreviewSessionStore sessions)
    {
        _sessions = sessions;
    }

    public async Task Handle(ChooseChoiceCommand command)
    {
        var session = await _sessions.GetByIDAsync(command.SessionId);

        session.Player.SelectChoice(command.Choice);

        session.Player.ExecuteNext();

        return await SceneSnapshot.From(session.Player, command.SessionId);
    }
}
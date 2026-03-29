using NoviVovi.Application.Labels;
using NoviVovi.Application.Preview.Contracts;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Preview.Services;

namespace NoviVovi.Application.Preview.Features.Choose;

public class ChooseChoiceHandler(
    PreviewSessionStore sessions,
    ILabelRepository labelRepository,
    SceneStateMapper stateMapper)
{
    public async Task<SceneStateSnapshot> Handle(ChooseChoiceCommand command)
    {
        var session = await sessions.GetByIdAsync(command.SessionId);
        if (session == null)
        {
            throw new Exception($"Session with id {command.SessionId} not found.");
        }

        session.Player.SelectChoice(command.ChoiceId);

        await session.Player.ExecuteNextAsync(labelRepository);

        return stateMapper.ToSnapshot(session.State);
    }
}
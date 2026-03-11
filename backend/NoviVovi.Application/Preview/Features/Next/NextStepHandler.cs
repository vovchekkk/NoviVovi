using NoviVovi.Application.Abstractions;
using NoviVovi.Application.Preview.Contracts;
using NoviVovi.Application.Preview.Mappers;
using NoviVovi.Application.Preview.Services;

namespace NoviVovi.Application.Preview.Features.Next;

public class NextStepHandler(
    PreviewSessionStore sessions,
    ILabelRepository labelRepository,
    SceneMapper mapper
)
{
    public async Task<SceneSnapshot> Handle(NextStepCommand command)
    {
        var session = await sessions.GetByIdAsync(command.SessionId);
        if (session == null)
        {
            throw new Exception($"Session with id {command.SessionId} not found.");
        }

        await session.Player.ExecuteNextAsync(labelRepository);
        
        await sessions.SaveAsync(session);

        return mapper.ToSnapshot(session.State);
    }
}
using NoviVovi.Domain.Novels;

namespace NoviVovi.Application.Preview;

public class PreviewSession
{
    public Guid Id { get; } = Guid.NewGuid();

    public SceneState State { get; }

    public ScenePlayer Player { get; }

    public PreviewSession(Novel novel)
    {
        State = new SceneState(novel);
        Player = new ScenePlayer(State);
    }
}